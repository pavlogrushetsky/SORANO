using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.ViewModels.DeliveryItem;

namespace SORANO.WEB.Components
{
    public class DeliveryItemsTableViewComponent : ViewComponent
    {
        private readonly IDeliveryItemService _deliveryItemService;
        private readonly IMapper _mapper;

        public DeliveryItemsTableViewComponent(IDeliveryItemService deliveryItemService, IMapper mapper)
        {
            _deliveryItemService = deliveryItemService;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync(int deliveryId)
        {
            var deliveryItems = await _deliveryItemService.GetForDeliveryAsync(deliveryId);
            var model = _mapper.Map<DeliveryItemTableViewModel>(deliveryItems.Result.Items);
            model.Summary = _mapper.Map<DeliveryItemsSummaryViewModel>(deliveryItems.Result.Summary);
            model.Mode = DeliveryItemTableMode.CreateDelivery;
            model.DeliveryId = deliveryId;

            return View(model);
        }
    }
}