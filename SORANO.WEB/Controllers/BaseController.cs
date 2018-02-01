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

        protected int UserId
        {
            get
            {
                var userResult = UserService.Get(HttpContext.User.FindFirst(ClaimTypes.Name)?.Value);
                return userResult.Status == ServiceResponseStatus.Success ? userResult.Result.ID : 0;
            }
        }

        protected int? LocationId
        {
            get
            {
                var locationId = HttpContext.User.FindFirst("LocationId")?.Value;
                return string.IsNullOrWhiteSpace(locationId)
                    ? (int?)null
                    : int.Parse(locationId);
            }
        }

        protected string LocationName
        {
            get
            {
                return HttpContext.User.FindFirst("LocationName")?.Value;
            }
        }

        public BaseController(IUserService userService)
        {
            UserService = userService;           
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
                // TODO Log in database
                result = onFault.Invoke("Не удалось выполнить операцию из-за непредвиденного исключения. Обратитесь к системному администратору.");
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