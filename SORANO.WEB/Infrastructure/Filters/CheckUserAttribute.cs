using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SORANO.BLL.Services;
using SORANO.BLL.Services.Abstract;

namespace SORANO.WEB.Infrastructure.Filters
{
    public class CheckUserAttribute : TypeFilterAttribute
    {
        public CheckUserAttribute() : base(typeof(CheckUserFilterImplementation))
        {            
        }

        private class CheckUserFilterImplementation : IAsyncActionFilter
        {
            private readonly IUserService _userService;

            public CheckUserFilterImplementation(IUserService userService)
            {
                _userService = userService;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var currentLocationClaim = context.HttpContext.User.FindFirst("LocationId")?.Value;
                var locationId = string.IsNullOrWhiteSpace(currentLocationClaim)
                    ? (int?)null
                    : int.Parse(currentLocationClaim);

                var userName = context.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

                var userResult = await _userService.GetAsync(userName, locationId);                            

                var isBlocked = userResult.Status != ServiceResponseStatus.Success
                                || userResult.Result == null
                                || userResult.Result.IsBlocked;

                if (isBlocked)
                    context.Result = new RedirectToActionResult("Logout", "Account", null);
                else
                {
                    await next();
                }
            }
        }
    }
}