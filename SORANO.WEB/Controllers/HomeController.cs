using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using System;
using System.Threading.Tasks;
using SORANO.WEB.ViewModels;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager,editor,user")]
    public class HomeController : Controller
    {
        private readonly IDeliveryService _deliveryService;
        private readonly IGoodsService _goodsService;

        public HomeController(IDeliveryService deliveryService, IGoodsService goodsService)
        {
            _deliveryService = deliveryService;
            _goodsService = goodsService;
        }

        public async Task<IActionResult> Index()
        {
            var deliveriesCount = await _deliveryService.GetSubmittedCountAsync();
            var totalIncome = await _goodsService.GetTotalIncomeAsync();

            return View(new DashboardModel
            {
                DeliveriesCount = deliveriesCount,
                TotalIncome = Math.Round(totalIncome).ToString() + " ₴"
            });
        }
    }
}
