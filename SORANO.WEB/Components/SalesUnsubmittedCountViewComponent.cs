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

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userResult = _userService.Get(HttpContext.User.FindFirst(ClaimTypes.Name)?.Value);
            var userId = userResult.Status == ServiceResponseStatus.Success ? userResult.Result.ID : 0;

            var locationIdStr = HttpContext.User.FindFirst("LocationId")?.Value;
            var locationId = string.IsNullOrWhiteSpace(locationIdStr)
                ? (int?)null
                : int.Parse(locationIdStr);

            var count = await _saleService.GetUnsubmittedCountAsync(locationId);

            return View(count.Result);
        }
    }
}
