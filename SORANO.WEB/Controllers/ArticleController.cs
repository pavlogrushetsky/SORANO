using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.WEB.Infrastructure.Extensions;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> All()
        {
            //var articles = await _articleService.GetAllWithTypeAsync();

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

            return Json(articles.ToList().Select(a => a.ToTableModel()));
        }
    }
}