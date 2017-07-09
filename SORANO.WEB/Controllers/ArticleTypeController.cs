using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Models.Article;
using SORANO.WEB.Models.ArticleType;
using Microsoft.Extensions.Caching.Memory;
using MimeTypes;
using SORANO.WEB.Models.Attachment;

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
            HttpContext.Session.SetBool("ShowDeletedArticleTypes", show);

            return RedirectToAction("Index", "Article");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = TempData.Get<ArticleTypeModel>("ArticleTypeModel") ?? new ArticleTypeModel
            {
                MainPicture = new AttachmentModel()
            };

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
            var articleType = await _articleTypeService.GetAsync(id);

            var model = TempData.Get<ArticleTypeModel>("ArticleTypeModel") ?? articleType.ToModel();

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = true;

            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Select(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                return BadRequest();
            }

            var typeModel = TempData.Get<ArticleTypeModel>("ArticleTypeModel");
            var articleModel = TempData.Get<ArticleModel>("ArticleModel");

            if (typeModel == null && articleModel == null)
            {
                return Redirect(returnUrl);
            }

            var types = await _articleTypeService.GetAllAsync(false);

            var model = new ArticleTypeSelectModel
            {
                Types = types.Select(t => t.ToModel()).ToList(),
                ArticleType = typeModel,
                Article = articleModel,
                ReturnUrl = returnUrl
            };

            if (typeModel != null)
            {
                model.Types.Where(t => t.ID == typeModel.ParentType?.ID)
                    .ToList()
                    .ForEach(t =>
                    {
                        t.IsSelected = true;
                    });
            }
            else
            {
                model.Types.Where(t => t.ID == articleModel.Type?.ID).ToList().ForEach(t =>
                {
                    t.IsSelected = true;
                });
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

            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor("MainPicture");
                attachmentTypes = await GetAttachmentTypes();
                ViewBag.AttachmentTypes = attachmentTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                for (var i = 0; i < model.Attachments.Count; i++)
                {
                    var extensions = model.Attachments[i]
                        .Type.MimeTypes.Split(',')
                        .Select(MimeTypeMap.GetExtension);

                    if (!extensions.Contains(Path.GetExtension(model.Attachments[i].FullPath)))
                    {
                        ModelState.AddModelError($"Attachments[{i}].Name", "Вложение не соответствует указанному типу");
                    }
                }

                // Check the model
                if (!ModelState.IsValid)
                {
                    ModelState.RemoveDuplicateErrorMessages();
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
                    return RedirectToAction("Index", "Article");
                }

                // If failed
                ModelState.AddModelError("", "Не удалось создать новый тип артикулов.");
                return View(model);
            }, ex => 
            {
                ViewBag.AttachmentTypes = attachmentTypes;
                ModelState.AddModelError("", ex);
                return View(model);
            });            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ArticleTypeModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();

            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor("MainPicture");
                attachmentTypes = await GetAttachmentTypes();
                ViewBag.AttachmentTypes = attachmentTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                for (var i = 0; i < model.Attachments.Count; i++)
                {
                    var extensions = model.Attachments[i]
                        .Type.MimeTypes.Split(',')
                        .Select(MimeTypeMap.GetExtension);

                    if (!extensions.Contains(Path.GetExtension(model.Attachments[i].FullPath)))
                    {
                        ModelState.AddModelError($"Attachments[{i}].Name", "Вложение не соответствует указанному типу");
                    }
                }

                // Check the model
                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    ModelState.RemoveDuplicateErrorMessages();
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
                ModelState.AddModelError("", ex);
                ViewData["IsEdit"] = true;
                return View("Create", model);
            });            
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ArticleTypeModel model)
        {
            var currentUser = await GetCurrentUser();

            await _articleTypeService.DeleteAsync(model.ID, currentUser.ID);

            return RedirectToAction("Index", "Article");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectParentType(ArticleTypeModel model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            await LoadMainPicture(model, mainPictureFile);
            await LoadAttachments(model, attachments);

            TempData.Put("ArticleTypeModel", model);

            return RedirectToAction("Select", "ArticleType", new { returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Select(ArticleTypeSelectModel model)
        {
            var typeModel = model.ArticleType;
            var articleModel = model.Article;

            var selectedType = model.Types.SingleOrDefault(t => t.IsSelected);

            if (typeModel != null)
            {
                typeModel.ParentType = selectedType;

                TempData.Put("ArticleTypeModel", typeModel);
            }
            else
            {
                articleModel.Type = selectedType;

                TempData.Put("ArticleModel", articleModel);
            }
            
            return Redirect(model.ReturnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(ArticleTypeSelectModel model)
        {
            if (model.Article != null)
            {
                TempData.Put("ArticleModel", model.Article);
            }

            if (model.ArticleType != null)
            {
                TempData.Put("ArticleTypeModel", model.ArticleType);
            }          

            return Redirect(model.ReturnUrl);
        }

        #endregion        
    }
}