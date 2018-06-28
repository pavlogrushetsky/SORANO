using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using System;
using System.Threading.Tasks;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager,editor,user")]
    [CheckUser]
    public class HomeController : BaseController
    {
        private readonly ILocationService _locationService;

        public HomeController(ILocationService locationService, 
            IUserService userService,
            IExceptionService exceptionService) 
            : base(userService, exceptionService)
        {
            _locationService = locationService;
        }

        public async Task<IActionResult> Index()
        {           
            var locationIdStr = HttpContext.User.FindFirst("LocationId")?.Value;
            var locationId = string.IsNullOrWhiteSpace(locationIdStr)
                ? (int?)null
                : int.Parse(locationIdStr);

            var summary = await _locationService.GetSummary(locationId);

            return View(new DashboardModel
            {
                TotalIncome = Format(summary.Result.TotalIncome),
                TotalSales = Format(summary.Result.TotalSales),
                Balance = Format(summary.Result.Balance),
                GoodsCount = summary.Result.GoodsCount,
                IsBalancePositive = summary.Result.IsBalancePositive
            });
        }

        private string Format(decimal value)
        {
            if (value > -1000.0M && value < 1000.0M)
                return Math.Round(value) + " ₴";

            if (value > -1000000.0M && value < 1000000.0M)
                return Math.Round(value / 1000) + " тыс. ₴";

            return Math.Round(value / 1000000) + " млн. ₴";                       
        }
    }
}
