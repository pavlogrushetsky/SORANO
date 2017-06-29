﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Models.Article;
using Microsoft.Extensions.Caching.Memory;
using SORANO.WEB.Models.Attachment;

namespace SORANO.WEB.Controllers
{
    /// <summary>
    /// Controller to handle requests to articles
    /// </summary>
    [Authorize(Roles = "developer,administrator,manager")]
    public class ArticleController : EntityBaseController<ArticleModel>
    {
        private readonly IArticleService _articleService;

        private readonly IHostingEnvironment _environment;

        /// <summary>
        /// Article controller
        /// </summary>
        /// <param name="articleService">Articles service</param>
        /// <param name="articleTypeService">Article types service</param>
        /// <param name="userService">Users service</param>
        public ArticleController(IArticleService articleService, 
            IArticleTypeService articleTypeService, 
            IUserService userService, 
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService, 
            IMemoryCache memoryCache) : base(userService, attachmentTypeService, memoryCache)
        {
            _articleService = articleService;
            _environment = environment;
        }

        #region GET Actions

        /// <summary>
        /// Get Index view
        /// </summary>
        /// <param name="withDeleted">Show deleted articles</param>
        /// <returns>Index view</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            bool showDeletedArticles = HttpContext.Session.GetBool("ShowDeletedArticles");
            bool showDeletedArticleTypes = HttpContext.Session.GetBool("ShowDeletedArticleTypes");

            var articles = await _articleService.GetAllAsync(showDeletedArticles);

            ViewBag.ShowDeletedArticles = showDeletedArticles;
            ViewBag.ShowDeletedArticleTypes = showDeletedArticleTypes;

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
            HttpContext.Session.SetBool("ShowDeletedArticles", show);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Get Create view
        /// </summary>
        /// <returns>Create view</returns>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = TempData.Get<ArticleModel>("ArticleModel") ?? new ArticleModel
            {
                MainPicture = new AttachmentModel()
            };

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            return View(model);
        }

        /// <summary>
        /// Get Update/Create view for specified article
        /// </summary>
        /// <param name="id">Article identifier</param>
        /// <param name="returnUrl"></param>
        /// <returns>Update/Create view</returns>
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var article = await _articleService.GetAsync(id);

            var model = TempData.Get<ArticleModel>("ArticleModel") ?? article.ToModel();

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = true;

            return View("Create", model);
        }

        /// <summary>
        /// Get Details view for specified article
        /// </summary>
        /// <param name="id">Article identifier</param>
        /// <param name="returnUrl"></param>
        /// <returns>Details view</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var article = await _articleService.GetAsync(id);

            var model = article.ToModel();

            return View(model);
        }

        /// <summary>
        /// Get Delete view for specified article
        /// </summary>
        /// <param name="id">Article identifier</param>
        /// <param name="returnUrl"></param>
        /// <returns>Delete view</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _articleService.GetAsync(id);

            var model = article.ToModel();

            return View(model);
        }        

        #endregion

        #region POST Actions

        /// <summary>
        /// Post article for redirection to article type selection view
        /// </summary>
        /// <param name="model">Article model</param>
        /// <param name="return">Return url</param>
        /// <returns>Article type Select view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectParentType(ArticleModel model, string returnUrl)
        {
            TempData.Put("ArticleModel", model);

            return RedirectToAction("Select", "ArticleType", new { returnUrl });
        }

        /// <summary>
        /// Post article for creation
        /// </summary>
        /// <param name="model">Article model</param>
        /// <returns>Index view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleModel model, IFormFile mainPictureFile)
        {
            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            return await TryGetActionResultAsync(async () =>
            {
                if (mainPictureFile != null)
                {
                    var extension = Path.GetExtension(mainPictureFile.FileName);
                    var path = "/attachments/temporary/" + Guid.NewGuid() + extension;

                    using (var stream = new FileStream(_environment.WebRootPath + path, FileMode.Create))
                    {
                        await mainPictureFile.CopyToAsync(stream);
                    }

                    model.MainPicture.FullPath = path;
                    model.MainPicture.Name = mainPictureFile.FileName;
                }

                ModelState.Keys.Where(k => k.StartsWith("Type")).ToList().ForEach(k =>
                {
                    ModelState.Remove(k);
                });

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
                ModelState.AddModelError("", ex);
                return View(model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ArticleModel model)
        {
            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            return await TryGetActionResultAsync(async () =>
            {
                ModelState.Keys.Where(k => k.StartsWith("Type")).ToList().ForEach(k =>
                {
                    ModelState.Remove(k);
                });

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