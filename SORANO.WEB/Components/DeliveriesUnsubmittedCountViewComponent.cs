using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using System.Threading.Tasks;

namespace SORANO.WEB.Components
{
    public class DeliveriesUnsubmittedCountViewComponent : ViewComponent
    {
        private readonly IDeliveryService _deliveryService;

        public DeliveriesUnsubmittedCountViewComponent(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var locationIdStr = HttpContext.User.FindFirst("LocationId")?.Value;
            var locationId = string.IsNullOrWhiteSpace(locationIdStr)
                ? (int?)null
                : int.Parse(locationIdStr);

            var count = await _deliveryService.GetUnsubmittedCountAsync(locationId);

            return View(count.Result);
        }
    }
}
