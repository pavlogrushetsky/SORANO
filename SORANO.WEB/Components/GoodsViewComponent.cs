using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Dtos;
using SORANO.BLL.Services;
using SORANO.BLL.Services.Abstract;
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

        public async Task<IViewComponentResult> InvokeAsync(GoodsIndexViewModel model)
        {
            var filterDto = _mapper.Map<GoodsFilterCriteriaDto>(model);

            var result = await _goodsService.GetAllAsync(filterDto);

            var viewModel = new List<GoodsItemViewModel>();

            if (result.Status != ServiceResponseStatus.Success)
                return View(viewModel);

            viewModel = _mapper.Map<List<GoodsItemViewModel>>(result.Result);
            return View(viewModel);
        }
    }
}