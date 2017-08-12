using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Models;
using System.Threading.Tasks;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager,editor,user")]
    public class HomeController : Controller
    {
        private readonly IDeliveryService _deliveryService;

        public HomeController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public async Task<IActionResult> Index()
        {
            var deliveriesCount = await _deliveryService.GetSubmittedCountAsync();

            return View(new DashboardModel
            {
                DeliveriesCount = deliveriesCount
            });
        }
    }
}
