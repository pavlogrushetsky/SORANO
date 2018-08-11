using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Dtos;
using SORANO.BLL.Services;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.ViewModels.Common;
using SORANO.WEB.ViewModels.Goods;

namespace SORANO.WEB.Components
{
    public class GoodsViewComponent : ViewComponent
    {
        private readonly IGoodsService _goodsService;
        private readonly IMapper _mapper;

        public GoodsViewComponent(IGoodsService goodsService, IMapper mapper)
        {
            _goodsService = goodsService;
            _mapper = mapper;
        }

#pragma warning disable 1998
        public async Task<IViewComponentResult> InvokeAsync(GoodsIndexViewModel model)
#pragma warning restore 1998
        {
            var filterDto = _mapper.Map<GoodsFilterCriteriaDto>(model);

            var result = _goodsService.GetAll(filterDto);

            var viewModel = new PaginationSet<GoodsItemViewModel>
            {
                Page = 1,
                TotalPages = 1,
                TotalCount = 0,
                Items = new List<GoodsItemViewModel>()
            };

            if (result.Status != ServiceResponseStatus.Success)
                return View(viewModel);

            viewModel = _mapper.Map<PaginationSet<GoodsItemViewModel>>(result.Result);
            return View(viewModel);
        }
    }
}