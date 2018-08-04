using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using System;
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

        public IActionResult Index()
        {
            return TryGetActionResult(() =>
            {
                var locationIdStr = HttpContext.User.FindFirst("LocationId")?.Value;
                var locationId = string.IsNullOrWhiteSpace(locationIdStr)
                    ? (int?)null
                    : int.Parse(locationIdStr);

                var summary = _locationService.GetSummary(locationId, UserId);

                return View(new DashboardModel
                {
                    MonthDeliveries = Format(summary.Result.MonthDeliveries),
                    MonthSales = Format(summary.Result.MonthSales),
                    MonthPersonalSales = Format(summary.Result.MonthPersonalSales),
                    MonthProfit = Format(summary.Result.MonthProfit),
                    GoodsCount = summary.Result.GoodsCount,
                    IsProfitPositive = summary.Result.IsProfitPositive,
                    IsSuperUser = !IsJustUser,
                    ShowForLocation = locationId.HasValue
                });
            }, BadRequest);           
        }

        private static string Format(decimal value)
        {
            if (value > -100000.0M && value < 100000.0M)
                return Math.Round(value) + " грн.";

            if (value > -1000000.0M && value < 1000000.0M)
                return Math.Round(value / 1000) + " тыс. грн.";

            return Math.Round(value / 1000000) + " млн. грн.";                       
        }
    }
}
