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
using SORANO.WEB.Models.ArticleType;

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

        #endregion

        #region POST Actions

        #endregion
    }
}