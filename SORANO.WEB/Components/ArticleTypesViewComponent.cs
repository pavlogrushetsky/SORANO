using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using System.Threading.Tasks;
using AutoMapper;
using SORANO.WEB.ViewModels.ArticleType;

namespace SORANO.WEB.Components
{
    public class ArticleTypesViewComponent : ViewComponent
    {
        private readonly IArticleTypeService _articleTypeService;
        private readonly IMapper _mapper;

        public ArticleTypesViewComponent(IArticleTypeService articleTypeService, IMapper mapper)
        {
            _articleTypeService = articleTypeService;
            _mapper = mapper;
        }

#pragma warning disable 1998
        public async Task<IViewComponentResult> InvokeAsync(bool showDeleted, string searchTerm)
#pragma warning restore 1998
        {
            var result = _articleTypeService.GetTree(showDeleted, searchTerm);

            var viewModel = _mapper.Map<ArticleTypeTreeViewModel>(result.Result);
            viewModel.ShowDeleted = showDeleted;
            viewModel.SearchTerm = searchTerm;

            return View(viewModel);
        }
    }
}
