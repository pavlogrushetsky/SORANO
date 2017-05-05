using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Models;
using SORANO.CORE.AccountEntities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer, administrator")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public UserController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
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

        public IActionResult Create()
        {
            var model = new UserModel();

            model.AllRoles = new List<SelectListItem>
            {
                new SelectListItem { Value = "developer", Text = "Разработчик" },
                new SelectListItem { Value = "administrator", Text = "Администратор" },
                new SelectListItem { Value = "editor", Text = "Редактор" },
                new SelectListItem { Value = "manager", Text = "Менеджер" },
                new SelectListItem { Value = "user", Text = "Пользователь" },
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User
            {
                Login = model.Login,
                Description = model.Description,
                Password = model.Password
            };

            var roles = await _roleService.GetAllAsync();

            model.Roles.ToList().ForEach(m =>
            {
                user.Roles.Add(roles.First(r => r.Name.Equals(m)));
            });

            user = await _userService.CreateAsync(user);

            if (user == null)
            {
                ModelState.AddModelError("Login", "Пользователь с таким логином уже существует в системе");
                return View(model);
            }
            else
            {
                return RedirectToAction("List", "Users");
            }          
        }
    }
}