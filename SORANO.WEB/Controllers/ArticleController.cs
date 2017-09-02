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
using SORANO.BLL.Services;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Models;

namespace SORANO.WEB.Controllers
{
    /// <summary>
    /// Controller to handle requests to articles
    /// </summary>
    [Authorize(Roles = "developer,administrator,manager")]
    public class ArticleController : EntityBaseController<ArticleModel>
    {
        private readonly IArticleService _articleService;
        private readonly IArticleTypeService _articleTypeService;

        public ArticleController(IArticleService articleService,
            IArticleTypeService articleTypeService,
            IUserService userService, 
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache) : base(userService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _articleService = articleService;
            _articleTypeService = articleTypeService;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await TryGetActionResultAsync(async () =>
            {
                var showDeletedArticles = Session.GetBool("ShowDeletedArticles");
                var showDeletedArticleTypes = Session.GetBool("ShowDeletedArticleTypes");

                var result = await _articleService.GetAllAsync(showDeletedArticles);

                if (result.Status == ServiceResponseStatusType.Fail)
                {
                    ViewBag.Error = result.Message;
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ShowDeletedArticles = showDeletedArticles;
                ViewBag.ShowDeletedArticleTypes = showDeletedArticleTypes;

                await ClearAttachments();

                return View(result.Result.Select(a => a.ToModel()).ToList());
            }, ex =>
            {
                ViewBag.Error = ex;
                return RedirectToAction("Index", "Home");
            });
        }

        [HttpGet]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedArticles", show);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create(string returnUrl)
        {
            return await TryGetActionResultAsync(async () =>
            {
                ArticleModel model;

                if (TryGetCached(out ArticleModel cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
                {
                    model = cachedForSelectMainPicture;
                    await CopyMainPicture(model);
                }
                else if (TryGetCached(out ArticleModel cachedForCreateType, CacheKeys.CreateArticleTypeCacheKey, CacheKeys.CreateArticleTypeCacheValidKey))
                {
                    model = cachedForCreateType;
                }
                else
                {
                    model = new ArticleModel
                    {
                        MainPicture = new AttachmentModel(),
                        ReturnPath = returnUrl
                    };
                }

                ViewBag.AttachmentTypes = await GetAttachmentTypes();
                ViewBag.ArticleTypes = await GetArticleTypes();

                return View(model);
            }, ex =>
            {
                ViewBag.Error = ex;
                return RedirectToAction("Index");
            });            
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                ArticleModel model;

                if (TryGetCached(out ArticleModel cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedForSelectMainPicture.ID == id)
                {
                    model = cachedForSelectMainPicture;
                    await CopyMainPicture(model);
                }
                else if (TryGetCached(out ArticleModel cachedForCreateType, CacheKeys.CreateArticleTypeCacheKey, CacheKeys.CreateArticleTypeCacheValidKey) && cachedForCreateType.ID == id)
                {
                    model = cachedForCreateType;
                }
                else
                {
                    var result = await _articleService.GetAsync(id);

                    if (result.Status == ServiceResponseStatusType.Fail)
                    {
                        ViewBag.Error = result.Message;
                        return RedirectToAction("Index");
                    }

                    model = result.Result.ToModel();
                }

                ViewBag.AttachmentTypes = await GetAttachmentTypes();
                ViewBag.ArticleTypes = await GetArticleTypes();

                ViewData["IsEdit"] = true;

                return View("Create", model);
            }, ex =>
            {
                ViewBag.Error = ex;
                return RedirectToAction("Index");
            });            
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _articleService.GetAsync(id);

                if (result.Status != ServiceResponseStatusType.Fail)
                    return View(result.Result.ToModel());

                ViewBag.Error = result.Message;
                return RedirectToAction("Index");
            }, ex =>
            {
                ViewBag.Error = ex;
                return RedirectToAction("Index");
            });            
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _articleService.GetAsync(id);

                if (result.Status != ServiceResponseStatusType.Fail)
                    return View(result.Result.ToModel());

                ViewBag.Error = result.Message;
                return RedirectToAction("Index");
            }, ex =>
            {
                ViewBag.Error = ex;
                return RedirectToAction("Index");
            });            
        }        

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateParentType(ArticleModel model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            await LoadMainPicture(model, mainPictureFile);
            await LoadAttachments(model, attachments);

            _memoryCache.Set(CacheKeys.CreateArticleTypeCacheKey, model);

            return RedirectToAction("Create", "ArticleType", new { returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();
            var articleTypes = new List<SelectListItem>();

            return await TryGetActionResultAsync(async () =>
            {
                attachmentTypes = await GetAttachmentTypes();
                articleTypes = await GetArticleTypes();

                ViewBag.AttachmentTypes = attachmentTypes;
                ViewBag.ArticleTypes = articleTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                var barcodeExists = await _articleService.BarcodeExistsAsync(model.Barcode);

                if (barcodeExists.Status == ServiceResponseStatusType.Fail)
                {
                    ViewBag.Error = barcodeExists.Message;
                    return RedirectToAction("Index");
                }

                if (barcodeExists.Result)
                {
                    ModelState.AddModelError("Barcode", "Артикул с таким штрих-кодом уже существует");
                }

                if (!ModelState.IsValid)
                {                   
                    return View(model);
                }

                var article = model.ToEntity();

                var currentUser = await GetCurrentUser();

                var result = await _articleService.CreateAsync(article, currentUser.ID);

                if (result.Status == ServiceResponseStatusType.Success && result.Result != null)
                {
                    if (string.IsNullOrEmpty(model.ReturnPath))
                    {
                        ViewBag.Success = $"Артикул \"{model.Name}\" был успешно создан";
                        return RedirectToAction("Index");
                    }

                    if (_memoryCache.TryGetValue(CacheKeys.CreateArticleCacheKey, out DeliveryModel cachedDelivery))
                    {
                        cachedDelivery.DeliveryItems[cachedDelivery.CurrentItemNumber].Article = model;
                        cachedDelivery.DeliveryItems[cachedDelivery.CurrentItemNumber].ArticleID = article.ID.ToString();
                        _memoryCache.Set(CacheKeys.CreateArticleCacheKey, cachedDelivery);
                        Session.SetBool(CacheKeys.CreateArticleCacheValidKey, true);
                    }
                    else
                    {
                        return BadRequest();
                    }

                    return Redirect(model.ReturnPath);
                }

                ViewBag.Error = result.Message;
                return View(model);
            }, ex =>
            {
                ViewBag.AttachmentTypes = attachmentTypes;
                ViewBag.ArticleTypes = articleTypes;

                ViewBag.Error = ex;
                return RedirectToAction("Index");
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ArticleModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();
            var articleTypes = new List<SelectListItem>();

            return await TryGetActionResultAsync(async () =>
            {
                attachmentTypes = await GetAttachmentTypes();
                articleTypes = await GetArticleTypes();

                ViewBag.AttachmentTypes = attachmentTypes;
                ViewBag.ArticleTypes = articleTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                var barcodeExists = await _articleService.BarcodeExistsAsync(model.Barcode, model.ID);

                if (barcodeExists.Status == ServiceResponseStatusType.Fail)
                {
                    ViewBag.Error = barcodeExists.Message;
                    return RedirectToAction("Index");
                }

                if (barcodeExists.Result)
                {
                    ModelState.AddModelError("Barcode", "Артикул с таким штрих-кодом уже существует");
                }

                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    return View("Create", model);
                }

                var article = model.ToEntity();

                var currentUser = await GetCurrentUser();

                var result = await _articleService.UpdateAsync(article, currentUser.ID);

                if (result.Status == ServiceResponseStatusType.Success)
                {
                    ViewBag.Success = $"Артикул \"{model.Name}\" был успешно обновлён";
                    return RedirectToAction("Index", "Article");
                }

                ViewBag.Error = result.Message;
                ViewData["IsEdit"] = true;
                return View("Create", model);
            }, ex =>
            {
                ViewBag.AttachmentTypes = attachmentTypes;
                ViewBag.ArticleTypes = articleTypes;

                ViewBag.Error = ex;
                return RedirectToAction("Index");
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ArticleModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var currentUser = await GetCurrentUser();

                await _articleService.DeleteAsync(model.ID, currentUser.ID);

                ViewBag.Success = $"Артикул \"{model.Name}\" был успешно помечен как удалённый";
                return RedirectToAction("Index", "Article");
            }, ex => RedirectToAction("Index", "Article"));            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(ArticleModel model)
        {
            if (string.IsNullOrEmpty(model.ReturnPath))
            {
                return RedirectToAction("Index", "Article");
            }

            if (_memoryCache.TryGetValue(CacheKeys.CreateArticleCacheKey, out DeliveryModel _))
            {
                Session.SetBool(CacheKeys.CreateArticleCacheValidKey, true);
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
                    Text = "-- Тип артикулов --"
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