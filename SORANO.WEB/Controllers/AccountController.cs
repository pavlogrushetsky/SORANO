using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.ViewModels.Account;
using SORANO.BLL.Services;
using SORANO.BLL.Dtos;

namespace SORANO.WEB.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        public AccountController(IUserService userService) : base(userService)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await UserService.GetAsync(model.Login, model.Password, model.LocationID);

            if (result.Status == ServiceResponseStatus.NotFound)
            {
                ModelState.AddModelError(nameof(model.Login), "Некорректные параметры входа");
                ModelState.AddModelError(nameof(model.Password), "Некорректные параметры входа");
                ModelState.AddModelError(nameof(model.LocationID), "Некорректные параметры входа");

                return View(model);
            }

            await Authenticate(result.Result, model.RememberMe, model.LocationID, model.LocationName);

            return string.IsNullOrEmpty(model.ReturnUrl) || model.ReturnUrl.Equals("/")
                ? RedirectToAction("Index", "Home")
                : (IActionResult)Redirect(model.ReturnUrl);           
        }

        private async Task Authenticate(UserDto user, bool rememberMe, int? locationId, string locationName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim("LocationId", locationId?.ToString() ?? string.Empty),
                new Claim("LocationName", locationName ?? string.Empty)
            };

            claims.AddRange(user.Roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name)));

            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.Authentication.SignInAsync("Cookies", new ClaimsPrincipal(id), new AuthenticationProperties { IsPersistent = rememberMe });
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(int id, string returnUrl = null)
        {
            var result = await UserService.GetAsync(id);
            if (result.Status != ServiceResponseStatus.Success || result.Result == null)
            {
                return NotFound();
            }

            return View(new ChangePasswordViewModel
            {
                Login = result.Result.Login,
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await UserService.GetAsync(model.Login, model.OldPassword);

            if (result.Status == ServiceResponseStatus.Success)
            {
                await UserService.ChangePasswordAsync(model.Login, model.NewPassword);

                return RedirectToAction("Login", new { model.ReturnUrl });
            }

            ModelState.AddModelError(nameof(model.OldPassword), "Пароль указан неверно");

            return View(model);
        }       
    }
}