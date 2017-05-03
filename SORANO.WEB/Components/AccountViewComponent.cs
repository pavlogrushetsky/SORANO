﻿using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Models;

namespace SORANO.WEB.Components
{
    public class AccountViewComponent : ViewComponent
    {
        private readonly IAccountService _accountService;

        public AccountViewComponent(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            var user = await _accountService.FindUserByLoginAsync(login);

            return new ViewViewComponentResult
            {
                ViewData = new ViewDataDictionary<AccountModel>(ViewData, new AccountModel
                {
                    ID = user.ID,
                    Description = user.Description,
                    Login = user.Login
                })
            };
        }
    }
}