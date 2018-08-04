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

#pragma warning disable 1998
        public async Task<IViewComponentResult> InvokeAsync(bool showDeleted, ArticleTableMode mode)
#pragma warning restore 1998
        {
            var articlesResult = _articleService.GetAll(showDeleted);
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