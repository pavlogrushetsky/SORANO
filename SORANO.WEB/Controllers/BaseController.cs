using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.AccountEntities;
using SORANO.BLL.Services;

namespace SORANO.WEB.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IUserService _userService;

        public BaseController(IUserService userService)
        {
            _userService = userService;
        }

        protected ISession Session => HttpContext.Session;

        protected async Task<int> GetCurrentUserId()
        {
            var result = await _userService.GetAsync(HttpContext.User.FindFirst(ClaimTypes.Name)?.Value);

            if (result.Status == ServiceResponseStatusType.Success)
                return result.Result.ID;

            return 0;
        }

        protected async Task<IActionResult> TryGetActionResultAsync(Func<Task<IActionResult>> function, Func<string, IActionResult> onFault)
        {
            IActionResult result;

            try
            {
                result = await function.Invoke();
            }
            catch (Exception ex)
            {
                result = onFault.Invoke(ex.Message);
            }

            return result;
        }

        protected IActionResult TryGetActionResult(Func<IActionResult> function, Func<string, IActionResult> onFault)
        {
            IActionResult result;

            try
            {
                result = function.Invoke();
            }
            catch (Exception ex)
            {
                result = onFault.Invoke(ex.Message);
            }

            return result;
        }
    }
}