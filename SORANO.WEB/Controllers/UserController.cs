using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Models;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer, administrator")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> List()
        {
            var users = await _userService.GetUsersAsync();

            var models = new List<UserModel>();

            users.ForEach(u =>
            {
                models.Add(new UserModel
                {
                    ID = u.ID,
                    Description = u.Description,
                    Login = u.Login,
                    IsBlocked = u.IsBlocked,
                    Roles = u.Roles.Select(r => r.Description).ToList()
                });
            });

            return View(models);
        }
    }
}