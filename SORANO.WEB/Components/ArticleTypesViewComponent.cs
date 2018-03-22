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

        public async Task<IViewComponentResult> InvokeAsync(bool showDeleted)
        {
            var result = await _articleTypeService.GetTreeAsync(showDeleted);

            var viewModel = _mapper.Map<ArticleTypeTreeViewModel>(result.Result);
            viewModel.ShowDeleted = showDeleted;

            return View(viewModel);
        }
    }
}
