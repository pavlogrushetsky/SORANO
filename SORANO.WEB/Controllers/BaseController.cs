using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.BLL.Services;

namespace SORANO.WEB.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IUserService UserService;

        protected int UserId { get; }

        public BaseController(IUserService userService)
        {
            UserService = userService;
            
            var userResult = UserService.Get(HttpContext.User.FindFirst(ClaimTypes.Name)?.Value);
            UserId = userResult.Status == ServiceResponseStatusType.Success ? userResult.Result.ID : 0;
        }

        protected ISession Session => HttpContext.Session;

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