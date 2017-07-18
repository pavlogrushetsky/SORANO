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
using Microsoft.Extensions.Caching.Memory;
using MimeTypes;
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

        /// <summary>
        /// Article controller
        /// </summary>
        /// <param name="articleService">Articles service</param>
        /// <param name="articleTypeService">Article types service</param>
        /// <param name="userService">Users service</param>
        /// <param name="environment"></param>
        /// <param name="attachmentTypeService"></param>
        /// <param name="attachmentService"></param>
        /// <param name="memoryCache"></param>
        public ArticleController(IArticleService articleService, 
            IArticleTypeService articleTypeService, 
            IUserService userService, 
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache) : base(userService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _articleService = articleService;
        }

        #region GET Actions

        /// <summary>
        /// Get Index view
        /// </summary>
        /// <returns>Index view</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var showDeletedArticles = Session.GetBool("ShowDeletedArticles");
            var showDeletedArticleTypes = Session.GetBool("ShowDeletedArticleTypes");

            var articles = await _articleService.GetAllAsync(showDeletedArticles);

            ViewBag.ShowDeletedArticles = showDeletedArticles;
            ViewBag.ShowDeletedArticleTypes = showDeletedArticleTypes;

            await ClearAttachments();

            return View(articles.Select(a => a.ToModel()).ToList());
        }

        /// <summary>
        /// Reload Index view with/without showing deleted articles
        /// </summary>
        /// <param name="show">If true show deleted articles</param>
        /// <returns>Redirection to Index view</returns>
        [HttpGet]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedArticles", show);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Get Create view
        /// </summary>
        /// <returns>Create view</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ArticleModel model;

            if (TryGetCached(out ArticleModel cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
            {
                model = cachedForSelectMainPicture;
                await CopyMainPicture(model);
            }
            else if (TryGetCached(out ArticleModel cachedForSelectType, CacheKeys.SelectArticleTypeCacheKey, CacheKeys.SelectArticleTypeCacheValidKey))
            {
                model = cachedForSelectType;
            }
            else if (TryGetCached(out ArticleModel cachedForCreateType, CacheKeys.CreateArticleTypeCacheKey, CacheKeys.CreateArticleTypeCacheValidKey))
            {
                model = cachedForCreateType;
            }
            else
            {
                model = new ArticleModel
                {
                    MainPicture = new AttachmentModel()
                };
            }
            
            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            return View(model);
        }

        /// <summary>
        /// Get Update/Create view for specified article
        /// </summary>
        /// <param name="id">Article identifier</param>
        /// <returns>Update/Create view</returns>
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            ArticleModel model;

            if (TryGetCached(out ArticleModel cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedForSelectMainPicture.ID == id)
            {
                model = cachedForSelectMainPicture;
                await CopyMainPicture(model);
            }
            else if (TryGetCached(out ArticleModel cachedForSelectType, CacheKeys.SelectArticleTypeCacheKey, CacheKeys.SelectArticleTypeCacheValidKey) && cachedForSelectType.ID == id)
            {
                model = cachedForSelectType;
            }
            else if (TryGetCached(out ArticleModel cachedForCreateType, CacheKeys.CreateArticleTypeCacheKey, CacheKeys.CreateArticleTypeCacheValidKey) && cachedForCreateType.ID == id)
            {
                model = cachedForCreateType;
            }
            else
            {
                var article = await _articleService.GetAsync(id);

                model = article.ToModel();
            }                

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = true;

            return View("Create", model);
        }

        /// <summary>
        /// Get Details view for specified article
        /// </summary>
        /// <param name="id">Article identifier</param>
        /// <returns>Details view</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var article = await _articleService.GetAsync(id);

            return View(article.ToModel());
        }

        /// <summary>
        /// Get Delete view for specified article
        /// </summary>
        /// <param name="id">Article identifier</param>
        /// <returns>Delete view</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _articleService.GetAsync(id);

            return View(article.ToModel());
        }        

        #endregion

        #region POST Actions

        /// <summary>
        /// Post article for redirection to article type selection view
        /// </summary>
        /// <param name="model">Article model</param>
        /// <param name="returnUrl"></param>
        /// <param name="mainPictureFile"></param>
        /// <param name="attachments"></param>
        /// <returns>Article type Select view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectParentType(ArticleModel model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            await LoadMainPicture(model, mainPictureFile);
            await LoadAttachments(model, attachments);

            _memoryCache.Set(CacheKeys.SelectArticleTypeCacheKey, model);

            return RedirectToAction("Select", "ArticleType", new { model.Type?.ID, returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateParentType(ArticleModel model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            await LoadMainPicture(model, mainPictureFile);
            await LoadAttachments(model, attachments);

            _memoryCache.Set(CacheKeys.CreateArticleTypeCacheKey, model);

            return RedirectToAction("Create", "ArticleType", new { returnUrl });
        }

        /// <summary>
        /// Post article for creation
        /// </summary>
        /// <param name="model">Article model</param>
        /// <param name="mainPictureFile"></param>
        /// <param name="attachments"></param>
        /// <returns>Index view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();

            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor("MainPicture");
                attachmentTypes = await GetAttachmentTypes();
                ViewBag.AttachmentTypes = attachmentTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);                               

                ModelState.RemoveFor("Type");

                for (var i = 0; i < model.Attachments.Count; i++)
                {
                    var extensions = model.Attachments[i]
                        .Type.MimeTypes?.Split(',')
                        .Select(MimeTypeMap.GetExtension);

                    if (extensions != null && !extensions.Contains(Path.GetExtension(model.Attachments[i].FullPath)))
                    {
                        ModelState.AddModelError($"Attachments[{i}].Name", "Вложение не соответствует указанному типу");
                    }
                }

                if (model.Type == null || model.Type.ID <= 0)
                {                   
                    ModelState.AddModelError("Type", "Необходимо указать родительский тип артикулов");
                }

                if (!ModelState.IsValid)
                {
                    ModelState.RemoveDuplicateErrorMessages();                    
                    return View(model);
                }

                var article = model.ToEntity();

                var currentUser = await GetCurrentUser();

                article = await _articleService.CreateAsync(article, currentUser.ID);

                if (article != null)
                {
                    return RedirectToAction("Index", "Article");
                }

                ModelState.AddModelError("", "Не удалось создать новый артикул.");
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
        public async Task<IActionResult> Update(ArticleModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();

            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor("MainPicture");
                attachmentTypes = await GetAttachmentTypes();
                ViewBag.AttachmentTypes = attachmentTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                ModelState.RemoveFor("Type");

                for (var i = 0; i < model.Attachments.Count; i++)
                {
                    var extensions = model.Attachments[i]
                        .Type.MimeTypes?.Split(',')
                        .Select(MimeTypeMap.GetExtension);

                    if (extensions != null && !extensions.Contains(Path.GetExtension(model.Attachments[i].FullPath)))
                    {
                        ModelState.AddModelError($"Attachments[{i}].Name", "Вложение не соответствует указанному типу");
                    }
                }

                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    ModelState.RemoveDuplicateErrorMessages();
                    return View("Create", model);
                }

                var article = model.ToEntity();

                var currentUser = await GetCurrentUser();

                article = await _articleService.UpdateAsync(article, currentUser.ID);

                if (article != null)
                {                    
                    return RedirectToAction("Index", "Article");
                }

                ModelState.AddModelError("", "Не удалось обновить артикул.");
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
        public async Task<IActionResult> Delete(ArticleModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var currentUser = await GetCurrentUser();

                await _articleService.DeleteAsync(model.ID, currentUser.ID);

                return RedirectToAction("Index", "Article");
            }, ex => RedirectToAction("Index", "Article"));            
        }        

        #endregion
    }
}