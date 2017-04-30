using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Models;

namespace SORANO.WEB.Components
{
    public class AccountViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IAccountService _accountService;

        public AccountViewComponent(IHttpContextAccessor httpContext, IAccountService accountService)
        {
            _httpContext = httpContext;
            _accountService = accountService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var email = _httpContext.HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            var user = await _accountService.FindUserByEmail(email);

            return View(new AccountModel
            {
                Name = user.Name
            });
        }
    }
}