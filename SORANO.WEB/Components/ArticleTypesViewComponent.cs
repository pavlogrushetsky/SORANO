using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace SORANO.WEB.Components
{
    public class ArticleTypesViewComponent : ViewComponent
    {
        private readonly IArticleTypeService _articleTypeService;

        public ArticleTypesViewComponent(IArticleTypeService articleTypeService)
        {
            _articleTypeService = articleTypeService;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool withDeleted = false)
        {
            var articleTypes = await _articleTypeService.GetAllAsync(withDeleted);

            return View(articleTypes.ToList().ToTree());
        }
    }
}
