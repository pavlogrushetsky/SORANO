﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Models.Article;
using SORANO.WEB.Models.Recommendation;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class ArticleController : BaseController
    {
        private readonly IArticleService _articleService;
        private readonly IArticleTypeService _articleTypeService;

        public ArticleController(IArticleService articleService, IArticleTypeService articleTypeService, IUserService userService) : base(userService)
        {
            _articleService = articleService;
            _articleTypeService = articleTypeService;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var types = await _articleTypeService.GetAllWithArticlesAsync();           

            return View(types.Select(t => t.ToModel()).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = TempData.Get<ArticleModel>("ArticleModel") ?? new ArticleModel(); 

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var article = await _articleService.GetAsync(id);

            var model = TempData.Get<ArticleModel>("ArticleModel") ?? article.ToModel(article.Type.ToModel(false));

            ViewData["IsEdit"] = true;

            return View("Create", model);
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectParentType(ArticleModel model, string returnUrl)
        {
            TempData.Put("ArticleModel", model);

            return RedirectToAction("Select", "ArticleType", new { returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleModel model)
        {
            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.Keys.Where(k => k.StartsWith("Type")).ToList().ForEach(k =>
                {
                    ModelState.Remove(k);
                });

                if (model.Type == null || model.Type.ID <= 0)
                {                   
                    ModelState.AddModelError("Type", "Необходимо указать родительский тип артикулов");
                }

                // Check the model
                if (!ModelState.IsValid)
                {
                    ModelState.RemoveDuplicateErrorMessages();
                    return View(model);
                }

                // Convert model to article entity
                var article = model.ToEntity();

                // Get current user
                var currentUser = await GetCurrentUser();

                // Call correspondent service method to create new article
                article = await _articleService.CreateAsync(article, currentUser.ID);

                // If succeeded
                if (article != null)
                {
                    return RedirectToAction("Index", "Article");
                }

                // If failed
                ModelState.AddModelError("", "Не удалось создать новый артикул.");
                return View(model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ArticleModel model)
        {
            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.Keys.Where(k => k.StartsWith("Type")).ToList().ForEach(k =>
                {
                    ModelState.Remove(k);
                });

                // Check the model
                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    ModelState.RemoveDuplicateErrorMessages();
                    return View("Create", model);
                }

                // Convert model to article type entity
                var article = model.ToEntity();

                // Get current user
                var currentUser = await GetCurrentUser();

                // Call correspondent service method to update article
                article = await _articleService.UpdateAsync(article, currentUser.ID);

                // If succeeded
                if (article != null)
                {
                    return RedirectToAction("Index", "Article");
                }

                // If failed
                ModelState.AddModelError("", "Не удалось обновить артикул.");
                ViewData["IsEdit"] = true;
                return View("Create", model);
            });
        }

        [HttpPost]
        public IActionResult AddRecommendation(ArticleModel article, bool isEdit)
        {
            ModelState.Clear();

            article.Recommendations.Add(new RecommendationModel());

            ViewData["IsEdit"] = isEdit;

            return View("Create", article);
        }

        [HttpPost]
        public IActionResult DeleteRecommendation(ArticleModel article, bool isEdit, int num)
        {
            ModelState.Clear();

            article.Recommendations.RemoveAt(num);

            ViewData["IsEdit"] = isEdit;

            return View("Create", article);
        }

        #endregion
    }
}