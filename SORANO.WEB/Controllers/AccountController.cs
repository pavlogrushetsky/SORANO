using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.AccountEntities;
using SORANO.WEB.Models;

namespace SORANO.WEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
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

                return RedirectToAction("Index", "Home");
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
        public async Task<IActionResult> ChangePassword(int id)
        {
            var user = await _accountService.FindUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(new ChangePasswordModel
            {
                Name = user.Name,
                Email = user.Email
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

                return RedirectToAction("Login");
            }

            ModelState.AddModelError(nameof(model.OldPassword), "Пароль указан неверно");

            return View(model);
        }       
    }
}