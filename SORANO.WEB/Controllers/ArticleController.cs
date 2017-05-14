using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public IActionResult Index()
        {
            var articles = new List<Article>
            {
                new Article
                {
                    ID = 1,
                    Code = "111111",
                    Name = "Test Article #1",
                    Description = "Test Article #1",
                    Type = new ArticleType
                    {
                        Name = "Test Article Type"
                    },
                    Producer = "Test Producer"
                }
            };

            var type1 = new ArticleType
            {
                Name = "Test article type #1"
            };

            var type2 = new ArticleType
            {
                Name = "Test article type #2"
            };

            var type3 = new ArticleType
            {
                Name = "Test article type #3"
            };

            type1.ChildTypes.Add(type2);

            type2.Articles.Add(new Article
            {
                Name = "Test article #1",
                Description = "Test article #1 description"
            });

            type2.Articles.Add(new Article
            {
                Name = "Test article #2",
                Description = "Test article #2 description"
            });

            type3.Articles.Add(new Article
            {
                Name = "Test article #3",
                Description = "Test article #3 description"
            });

            type3.Articles.Add(new Article
            {
                Name = "Test article #4",
                Description = "Test article #4 description"
            });

            var model = new ArticleIndexModel
            {
                Tree = new List<ArticleType>
                {
                    type1, type3
                },
                Table = articles.ToList().Select(a => a.ToTableModel())
            };

            return View(model);
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

            var model = new ArticleTypeCreateModel();

            model.AllTypes.Add(new SelectListItem
            {
                Value = "0",
                Selected = true,
                Disabled = true,
                Text = "Выберите один из типов"
            });

            model.AllTypes.AddRange(articleTypes.Select(t => new SelectListItem
            {
                Value = t.ID.ToString(),
                Text = t.Name
            }).ToList());

            return View(model);
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArticleType(ArticleTypeCreateModel model)
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