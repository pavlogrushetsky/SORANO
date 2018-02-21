using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.ViewModels.Sale;

namespace SORANO.WEB.Components
{
    public class SaleItemsViewComponent : ViewComponent
    {
        private readonly ISaleService _saleService;
        private readonly IMapper _mapper;

        public SaleItemsViewComponent(IMapper mapper, ISaleService saleService)
        {
            _saleService = saleService;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync(int saleId, int locationId, bool selectedOnly)
        {
            var result = await _saleService.GetItemsAsync(saleId, locationId, selectedOnly);

            var viewModel = new SaleItemsGroupsViewModel
            {
                Summary = new SaleItemsSummaryViewModel
                {
                    SelectedCount = 0,
                    TotalPrice = "0.00"
                }
            };         

            switch (result.Status)
            {
                case ServiceResponseStatus.InvalidOperation:
                    viewModel.Warning = "Необходимо указать магазин для получения списка товаров.";
                    break;
                case ServiceResponseStatus.NotFound:
                    viewModel.Warning = "В данном магазине товары отсутствуют.";
                    break;
                default:
                    viewModel = _mapper.Map<SaleItemsGroupsViewModel>(result.Result);
                    break;
            }

            return View(viewModel);
        }
    }
}