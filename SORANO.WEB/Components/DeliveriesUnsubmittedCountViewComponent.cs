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

#pragma warning disable 1998
        public async Task<IViewComponentResult> InvokeAsync()
#pragma warning restore 1998
        {
            var locationIdStr = HttpContext.User.FindFirst("LocationId")?.Value;
            var locationId = string.IsNullOrWhiteSpace(locationIdStr)
                ? (int?)null
                : int.Parse(locationIdStr);

            var count = _deliveryService.GetUnsubmittedCount(locationId);

            return View(count.Result);
        }
    }
}
