using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.AccountEntities;
using SORANO.WEB.Models;

namespace SORANO.WEB.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
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
            var user = await _accountService.GetUserAsync(model.Email, model.Password);

            if (user != null)
            {
                await Authenticate(user, model.RememberMe);

                return string.IsNullOrEmpty(model.ReturnUrl) || model.ReturnUrl.Equals("/")
                    ? RedirectToAction("Index", "Home")
                    : (IActionResult)Redirect(model.ReturnUrl);
            }

            ModelState.AddModelError(nameof(model.Email), "Некорректные логин и/или пароль");
            ModelState.AddModelError(nameof(model.Password), string.Empty);

            return View(model);
        }

        private async Task Authenticate(User user, bool rememberMe)
        {
            // Add clame for user name
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email)
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
            var user = await _accountService.FindUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(new ChangePasswordModel
            {
                Name = user.Name,
                Email = user.Email,
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

            var user = await _accountService.GetUserAsync(model.Email, model.OldPassword);

            if (user != null)
            {
                await _accountService.ChangePasswordAsync(model.Email, model.NewPassword);

                return RedirectToAction("Login", new { model.ReturnUrl });
            }

            ModelState.AddModelError(nameof(model.OldPassword), "Пароль указан неверно");

            return View(model);
        }       
    }
}