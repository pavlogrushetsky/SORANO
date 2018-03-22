using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.ViewModels.Article;

namespace SORANO.WEB.Components
{
    public class ArticleTableViewComponent : ViewComponent
    {
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;

        public ArticleTableViewComponent(IArticleService articleService, IMapper mapper)
        {
            _articleService = articleService;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool showDeleted, ArticleTableMode mode)
        {
            var articlesResult = await _articleService.GetAllAsync(showDeleted);
            if (articlesResult.Status != ServiceResponseStatus.Success)
            {
                
            }

            var viewModel = _mapper.Map<ArticleTableViewModel>(articlesResult.Result);
            viewModel.Mode = mode;
            viewModel.ShowDeleted = showDeleted;

            return View(viewModel);
        }
    }
}