using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.AccountEntities;
using Microsoft.AspNetCore.Mvc.Rendering;
using SORANO.WEB.Models.User;
using SORANO.WEB.Infrastructure.Extensions;
using System.Security.Claims;

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

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = await _userService.GetAllIncludeAllAsync();

            var models = new List<UserListModel>();

            users.ForEach(u => 
            {
                var isCurrent = u.IsCurrent(HttpContext);
                var hasActivities = u.HasActivities();
                models.Add(u.ToListModel(isCurrent, hasActivities));
            });

            return View(models);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetIncludeAllAsync(id);

            return View(user.ToDeleteModel(user.IsCurrent(HttpContext), user.HasActivities()));
        }

        [HttpGet]
        public async Task<IActionResult> Block(int id)
        {
            var user = await _userService.GetAsync(id);

            return View(user.ToBlockModel(user.IsCurrent(HttpContext)));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var user = await _userService.GetIncludeRolesAsync(id);

            return View(user.ToUpdateModel());
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([Bind("ID")]UserDeleteModel model)
        {
            await _userService.DeleteAsync(model.ID);

            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block([Bind("ID")]UserDeleteModel model)
        {
            var user = await _userService.GetAsync(model.ID);

            user.IsBlocked = !user.IsBlocked;

            await _userService.UpdateAsync(user);

            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([Bind("ID,Login,Description,NewPassword,RepeatPassword,Roles")]UserUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userService.GetAsync(model.ID);

            var roles = await _roleService.GetAllAsync();

            user.FromUpdateModel(model, roles.ToList());

            await _userService.UpdateAsync(user);

            return RedirectToAction("List");
        }

        #endregion

        public IActionResult Create()
        {
            var model = new UserCreateModel
            {
                AllRoles = new List<SelectListItem>
                {
                    new SelectListItem {Value = "developer", Text = "Разработчик"},
                    new SelectListItem {Value = "administrator", Text = "Администратор"},
                    new SelectListItem {Value = "editor", Text = "Редактор"},
                    new SelectListItem {Value = "manager", Text = "Менеджер"},
                    new SelectListItem {Value = "user", Text = "Пользователь"},
                }
            };


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateModel model)
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

            if (user != null)
            {
                return RedirectToAction("List", "User");
            }

            ModelState.AddModelError("Login", "Пользователь с таким логином уже существует в системе");
            return View(model);
        }        
    }
}