using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using System.Threading.Tasks;
using SORANO.BLL.Services;

namespace SORANO.WEB.Components
{
    public class SalesUnsubmittedCountViewComponent : ViewComponent
    {
        private readonly ISaleService _saleService;
        private readonly IUserService _userService;

        public SalesUnsubmittedCountViewComponent(ISaleService saleService, IUserService userService)
        {
            _saleService = saleService;
            _userService = userService;
        }

#pragma warning disable 1998
        public async Task<IViewComponentResult> InvokeAsync()
#pragma warning restore 1998
        {
            var userResult = _userService.Get(HttpContext.User.FindFirst(ClaimTypes.Name)?.Value);
            var userId = userResult.Status == ServiceResponseStatus.Success ? userResult.Result.ID : 0;

            var locationIdStr = HttpContext.User.FindFirst("LocationId")?.Value;
            var locationId = string.IsNullOrWhiteSpace(locationIdStr)
                ? (int?)null
                : int.Parse(locationIdStr);

            var count = _saleService.GetUnsubmittedCount(locationId);

            return View(count.Result);
        }
    }
}
