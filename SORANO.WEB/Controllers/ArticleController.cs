using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Models.Article;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IArticleTypeService _articleTypeService;
        private readonly IUserService _userService;

        public ArticleController(IArticleService articleService, IArticleTypeService articleTypeService, IUserService userService)
        {
            _articleService = articleService;
            _articleTypeService = articleTypeService;
            _userService = userService;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var types = await _articleTypeService.GetAllWithArticlesAsync();           

            return View(types.Select(t => t.ToModel()).ToList());
        }

        [HttpGet]
        public IActionResult CreateArticle()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateArticleType()
        {
            var types = await _articleTypeService.GetAllAsync();

            var articleTypes = types as IList<ArticleType> ?? types.ToList();

            var model = new ArticleTypeModel();

            //model.AllTypes.Add(new SelectListItem
            //{
            //    Value = "0",
            //    Selected = true,
            //    Disabled = true,
            //    Text = "Выберите один из типов"
            //});

            //model.AllTypes.AddRange(articleTypes.Select(t => new SelectListItem
            //{
            //    Value = t.ID.ToString(),
            //    Text = t.Name
            //}).ToList());

            return View(model);
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArticleType(ArticleTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var articleType = new ArticleType();

            articleType.FromCreateModel(model);

            var currentUser = await _userService.GetAsync(HttpContext.User.FindFirst(ClaimTypes.Name)?.Value);

            articleType.CreatedBy = currentUser.ID;
            articleType.ModifiedBy = currentUser.ID;

            articleType = await _articleTypeService.CreateAsync(articleType);

            if (articleType != null)
            {
                return RedirectToAction("Index", "Article");
            }

            ModelState.AddModelError("", "Не удалось создать новый тип артикулов.");
            return View(model);
        }

        #endregion
    }
}