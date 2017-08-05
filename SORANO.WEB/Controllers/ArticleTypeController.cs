using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Memory;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Models;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class ArticleTypeController : EntityBaseController<ArticleTypeModel>
    {
        private readonly IArticleTypeService _articleTypeService;

        public ArticleTypeController(IArticleTypeService articleTypeService, 
            IUserService userService,
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache) : base(userService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _articleTypeService = articleTypeService;
        }

        #region GET Actions

        [HttpGet]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedArticleTypes", show);

            return RedirectToAction("Index", "Article");
        }

        [HttpGet]
        public async Task<IActionResult> Create(string returnUrl)
        {
            ArticleTypeModel model;

            if (TryGetCached(out ArticleTypeModel cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
            {
                model = cachedForSelectMainPicture;
                await CopyMainPicture(model);
            }
            else
            {
                model = new ArticleTypeModel
                {
                    MainPicture = new AttachmentModel(),
                    ReturnPath = returnUrl                    
                };
            }

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Brief(int id)
        {
            var type = await _articleTypeService.GetAsync(id);

            await ClearAttachments();

            return PartialView("_Brief", type.ToModel());
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            ArticleTypeModel model;

            if (TryGetCached(out ArticleTypeModel cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedForSelectMainPicture.ID == id)
            {
                model = cachedForSelectMainPicture;
                await CopyMainPicture(model);
            }
            else
            {
                var articleType = await _articleTypeService.GetAsync(id);

                model = articleType.ToModel();
            }

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = true;

            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Select(int? selectedId, string returnUrl, int? currentTypeId)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                return BadRequest();
            }

            var types = await _articleTypeService.GetAllAsync(false);

            var model = new ArticleTypeSelectModel
            {
                Types = types.Select(t => t.ToModel()).ToList(),
                ReturnUrl = returnUrl
            };

            if (selectedId.HasValue)
            {
                var selectedType = model.Types.FirstOrDefault(t => t.ID == selectedId);
                if (selectedType != null)
                {
                    selectedType.IsSelected = true;
                }
            }

            if (currentTypeId.HasValue)
            {
                model.CurrentType = model.Types.FirstOrDefault(t => t.ID == currentTypeId);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var articleType = await _articleTypeService.GetAsync(id);

            return View(articleType.ToModel());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var articleType = await _articleTypeService.GetAsync(id);

            return View(articleType.ToModel());
        }

        #endregion

        #region POST Actions

        /// <summary>
        /// Create new article type
        /// </summary>
        /// <param name="model">Article type model</param>
        /// <param name="mainPictureFile"></param>
        /// <param name="attachments"></param>
        /// <returns>Action result</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleTypeModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();
            var articleTypes = new List<SelectListItem>();

            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                attachmentTypes = await GetAttachmentTypes();
                articleTypes = await GetArticleTypes();

                ViewBag.AttachmentTypes = attachmentTypes;
                ViewBag.ArticleTypes = articleTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                // Check the model
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Convert model to article type entity
                var articleType = model.ToEntity();

                // Get current user
                var currentUser = await GetCurrentUser();

                // Call correspondent service method to create new article type
                articleType = await _articleTypeService.CreateAsync(articleType, currentUser.ID);

                // If succeeded
                if (articleType != null)
                {
                    if (string.IsNullOrEmpty(model.ReturnPath))
                    {
                        return RedirectToAction("Index", "Article");
                    }

                    if (_memoryCache.TryGetValue(CacheKeys.CreateArticleTypeCacheKey, out ArticleModel cachedArticle))
                    {
                        cachedArticle.Type = articleType.ToModel();
                        _memoryCache.Set(CacheKeys.CreateArticleTypeCacheKey, cachedArticle);
                        Session.SetBool(CacheKeys.CreateArticleTypeCacheValidKey, true);
                    }
                    else
                    {
                        return BadRequest();
                    }

                    return Redirect(model.ReturnPath);
                }

                // If failed
                ModelState.AddModelError("", "Не удалось создать новый тип артикулов.");
                return View(model);
            }, ex => 
            {
                ViewBag.AttachmentTypes = attachmentTypes;
                ViewBag.ArticleTypes = articleTypes;

                ModelState.AddModelError("", ex);
                return View(model);
            });            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ArticleTypeModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();
            var articleTypes = new List<SelectListItem>();

            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                attachmentTypes = await GetAttachmentTypes();
                articleTypes = await GetArticleTypes();

                ViewBag.AttachmentTypes = attachmentTypes;
                ViewBag.ArticleTypes = articleTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                // Check the model
                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    return View("Create", model);
                }

                // Convert model to article type entity
                var articleType = model.ToEntity();

                // Get current user
                var currentUser = await GetCurrentUser();

                // Call correspondent service method to update article type
                articleType = await _articleTypeService.UpdateAsync(articleType, currentUser.ID);

                // If succeeded
                if (articleType != null)
                {
                    return RedirectToAction("Index", "Article");
                }

                // If failed
                ModelState.AddModelError("", "Не удалось обновить тип артикулов.");
                ViewData["IsEdit"] = true;
                return View("Create", model);
            }, ex => 
            {
                ViewBag.AttachmentTypes = attachmentTypes;
                ViewBag.ArticleTypes = articleTypes;

                ModelState.AddModelError("", ex);
                ViewData["IsEdit"] = true;
                return View("Create", model);
            });            
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ArticleTypeModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var currentUser = await GetCurrentUser();

                await _articleTypeService.DeleteAsync(model.ID, currentUser.ID);

                return RedirectToAction("Index", "Article");
            }, ex => RedirectToAction("Index", "Article"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(ArticleTypeModel model)
        {
            if (string.IsNullOrEmpty(model.ReturnPath))
            {
                return RedirectToAction("Index", "Article");
            }

            if (_memoryCache.TryGetValue(CacheKeys.CreateArticleTypeCacheKey, out ArticleModel _))
            {
                Session.SetBool(CacheKeys.CreateArticleTypeCacheValidKey, true);
            }

            return Redirect(model.ReturnPath);
        }

        #endregion

        private async Task<List<SelectListItem>> GetArticleTypes()
        {
            var articleTypes = await _articleTypeService.GetAllAsync(false);

            var articleTypeItems = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "0",
                    Text = "-- Тип артикула --"
                }
            };

            articleTypeItems.AddRange(articleTypes.Select(t => new SelectListItem
            {
                Value = t.ID.ToString(),
                Text = t.Name
            }));

            _memoryCache.Set(CacheKeys.ArticleTypesCacheKey, articleTypeItems);

            return articleTypeItems;
        }
    }
}