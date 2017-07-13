using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace SORANO.WEB.Components
{
    /// <summary>
    /// View component for rendering article types tree view
    /// </summary>
    public class ArticleTypesViewComponent : ViewComponent
    {
        private readonly IArticleTypeService _articleTypeService;

        /// <summary>
        /// View component for rendering article types tree view
        /// </summary>
        /// <param name="articleTypeService">Article types service</param>
        public ArticleTypesViewComponent(IArticleTypeService articleTypeService)
        {
            _articleTypeService = articleTypeService;
        }

        /// <summary>
        /// Invoke component asynchronously
        /// </summary>
        /// <param name="withDeleted">Show deleted article types</param>
        /// <returns>Component's default view</returns>
        public async Task<IViewComponentResult> InvokeAsync(bool withDeleted = false)
        {
            var articleTypes = await _articleTypeService.GetAllAsync(withDeleted);

            return View(articleTypes.Select(t => t.ToModel()).ToList().ToTree());
        }
    }
}
