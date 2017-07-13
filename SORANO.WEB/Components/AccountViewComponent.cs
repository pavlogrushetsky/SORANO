using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Models.Account;

namespace SORANO.WEB.Components
{
    /// <summary>
    /// View component for rendering login view
    /// </summary>
    public class AccountViewComponent : ViewComponent
    {
        private readonly IUserService _userService;

        /// <summary>
        /// /// <summary>
        /// View component for rendering Login view
        /// </summary>
        /// </summary>
        /// <param name="userService">User service</param>
        public AccountViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Invoke component asynchronously
        /// </summary>
        /// <returns>Component's default view</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            var user = await _userService.GetAsync(login);

            return View(new AccountModel
            {
                ID = user.ID,
                Description = user.Description,
                Login = user.Login
            });
        }
    }
}