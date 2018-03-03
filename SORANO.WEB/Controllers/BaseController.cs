using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Dtos;
using SORANO.BLL.Services.Abstract;
using SORANO.BLL.Services;

namespace SORANO.WEB.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IUserService UserService;
        protected readonly IExceptionService ExceptionService;

        private const string ExceptionMessage = "Не удалось выполнить операцию из-за непредвиденного исключения. Обратитесь к системному администратору.";

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

        protected bool IsDeveloper => HttpContext.User.IsInRole("developer");

        protected bool IsAdministrator => HttpContext.User.IsInRole("administrator");

        protected bool IsManager => HttpContext.User.IsInRole("manager");

        protected bool AllowCreation => IsAdministrator || IsManager || IsDeveloper;

        protected string LocationName => HttpContext.User.FindFirst("LocationName")?.Value;

        public BaseController(IUserService userService, IExceptionService exceptionService)
        {
            UserService = userService;
            ExceptionService = exceptionService;
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
                await ExceptionService.SaveAsync(new ExceptionDto
                {
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace,
                    Timestamp = DateTime.Now
                });

                result = onFault.Invoke(ExceptionMessage);
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
                ExceptionService.Save(new ExceptionDto
                {
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace,
                    Timestamp = DateTime.Now
                });

                result = onFault.Invoke(ExceptionMessage);
            }

            return result;
        }
    }
}