using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.AccountEntities;
using SORANO.WEB.ViewModels;

namespace SORANO.WEB.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userService.GetAsync(model.Login, model.Password);

            if (user != null)
            {
                await Authenticate(user, model.RememberMe);

                return string.IsNullOrEmpty(model.ReturnUrl) || model.ReturnUrl.Equals("/")
                    ? RedirectToAction("Index", "Home")
                    : (IActionResult)Redirect(model.ReturnUrl);
            }

            ModelState.AddModelError(nameof(model.Login), "Некорректные логин и/или пароль");
            ModelState.AddModelError(nameof(model.Password), "Некорректные логин и/или пароль");

            return View(model);
        }

        private async Task Authenticate(User user, bool rememberMe)
        {
            // Add clame for user name
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login)
            };

            // For each user role add new claim
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
            var user = await _userService.GetAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(new ChangePasswordModel
            {
                Description = user.Description,
                Login = user.Login,
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userService.GetAsync(model.Login, model.OldPassword);

            if (user != null)
            {
                await _userService.ChangePasswordAsync(model.Login, model.NewPassword);

                return RedirectToAction("Login", new { model.ReturnUrl });
            }

            ModelState.AddModelError(nameof(model.OldPassword), "Пароль указан неверно");

            return View(model);
        }       
    }
}