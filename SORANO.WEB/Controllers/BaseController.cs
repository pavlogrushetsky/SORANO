using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.AccountEntities;

namespace SORANO.WEB.Controllers
{
    public class BaseController : Controller
    {
        private readonly IUserService _userService;

        public BaseController(IUserService userService)
        {
            _userService = userService;
        }

        protected async Task<User> GetCurrentUser()
        {
            return await _userService.GetAsync(HttpContext.User.FindFirst(ClaimTypes.Name)?.Value);
        }

        protected async Task<IActionResult> TryGetActionResultAsync(Func<Task<IActionResult>> function)
        {
            IActionResult result;

            try
            {
                result = await function.Invoke();
            }
            catch (Exception)
            {
                result = new BadRequestResult();
            }

            return result;
        }

        protected IActionResult TryGetActionResult(Func<IActionResult> function)
        {
            IActionResult result;

            try
            {
                result = function.Invoke();
            }
            catch (Exception)
            {
                result = new BadRequestResult();
            }

            return result;
        }
    }
}