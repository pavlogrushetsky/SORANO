using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.AccountEntities;

namespace SORANO.WEB.Controllers
{
    /// <summary>
    /// Controller to perform controllers' base functionality
    /// </summary>
    public class BaseController : Controller
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Controller to perform controllers' base functionality
        /// </summary>
        /// <param name="userService">Users' service</param>
        public BaseController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get currently logged in user
        /// </summary>
        /// <returns></returns>
        protected async Task<User> GetCurrentUser()
        {
            return await _userService.GetAsync(HttpContext.User.FindFirst(ClaimTypes.Name)?.Value);
        }

        /// <summary>
        /// Try to perform controller logic asynchronously
        /// </summary>
        /// <param name="function">Function to try to perform</param>
        /// <param name="onFault">Function to perform if failed</param>
        /// <returns>Result</returns>
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

        /// <summary>
        /// Try to perform controller logic
        /// </summary>
        /// <param name="function">Function to try to perform</param>
        /// <param name="onFault">Function to perform if failed</param>
        /// <returns>Result</returns>
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