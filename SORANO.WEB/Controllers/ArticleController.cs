using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Models.Article;

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
        public ArticleController(IArticleService articleService, IArticleTypeService articleTypeService, IUserService userService) : base(userService)
        {
            _articleService = articleService;
        }

        #region GET Actions

        /// <summary>
        /// Get Index view
        /// </summary>
        /// <param name="withDeleted">Show deleted articles</param>
        /// <returns>Index view</returns>
        [HttpGet]
        public async Task<IActionResult> Index(bool withDeleted = false)
        {
            var articles = await _articleService.GetAllAsync(withDeleted);

            ViewBag.WithDeleted = withDeleted;

            return View(articles.Select(a => a.ToModel()).ToList());
        }

        /// <summary>
        /// Get Create view
        /// </summary>
        /// <returns>Create view</returns>
        [HttpGet]
        public IActionResult Create(string returnUrl = null)
        {
            var model = TempData.Get<ArticleModel>("ArticleModel") ?? new ArticleModel();
            model.ReturnUrl = returnUrl;

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
            var article = await _articleService.GetAsync(id);

            var model = TempData.Get<ArticleModel>("ArticleModel") ?? article.ToModel();

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
        /// <param name="returnUrl">Return url</param>
        /// <returns>Article type Select view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectParentType(ArticleModel model, string @return)
        {
            TempData.Put("ArticleModel", model);

            return RedirectToAction("Select", "ArticleType", new { @return });
        }

        /// <summary>
        /// Post article for creation
        /// </summary>
        /// <param name="model">Article model</param>
        /// <returns>Index view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleModel model)
        {
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
                    return Redirect(model.ReturnUrl);
                }

                ModelState.AddModelError("", "Не удалось создать новый артикул.");
                return View(model);
            }, (ex) => 
            {
                ModelState.AddModelError("", ex);
                return View(model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ArticleModel model)
        {
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
            }, (ex) =>
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
            }, (ex) => 
            {
                return RedirectToAction("Index", "Article");
            });            
        }        

        #endregion
    }
}