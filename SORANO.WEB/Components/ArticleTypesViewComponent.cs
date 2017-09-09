using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using System.Linq;
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

        public async Task<IViewComponentResult> InvokeAsync(bool withDeleted = false)
        {
            var result = await _articleTypeService.GetAllAsync(withDeleted);

            var viewModels = _mapper.Map<IEnumerable<ArticleTypeIndexViewModel>>(result.Result);

            return View(viewModels.ToList().ToTree());
        }
    }
}
