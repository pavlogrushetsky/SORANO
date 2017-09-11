using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.ViewModels;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer, administrator")]
    public class UserController : BaseController
    {
        private readonly IRoleService _roleService;

        public UserController(IUserService userService, IRoleService roleService) : base(userService)
        {
            _roleService = roleService;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = await UserService.GetAllAsync();

            var models = new List<UserModel>();

            users.ForEach(u =>
            {
                models.Add(u.ToModel(u.ID == UserId));
            });

            return View(models);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await UserService.GetAsync(id);

            return View(user.ToModel(user.ID == UserId));
        }

        [HttpGet]
        public async Task<IActionResult> Block(int id)
        {
            var user = await UserService.GetAsync(id);

            return View(user.ToModel(user.ID == UserId));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {           
            var user = await UserService.GetAsync(id);

            var model = user.ToModel(user.ID == UserId);

            var roles = await _roleService.GetAllAsync();

            var userRoles = roles.Select(r => new SelectListItem
            {
                Value = r.ID.ToString(),
                Text = r.Description,
                Selected = user.Roles.Select(x => x.ID).Contains(r.ID)
            });

            ViewBag.Roles = userRoles;

            ViewData["IsEdit"] = true;

            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleService.GetAllAsync();

            var userRoles = roles.Select(r => new SelectListItem
            {
                Value = r.ID.ToString(),
                Text = r.Description
            });

            ViewBag.Roles = userRoles;

            return View(new UserModel());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await UserService.GetAsync(id);

            return View(user.ToModel(user.ID == UserId));
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([Bind("ID")]UserModel model)
        {
            await UserService.DeleteAsync(model.ID);

            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block([Bind("ID")]UserModel model)
        {
            var user = await UserService.GetAsync(model.ID);

            user.IsBlocked = !user.IsBlocked;

            await UserService.UpdateAsync(user);

            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var roles = await _roleService.GetAllAsync();

                var userRoles = roles.Select(r => new SelectListItem
                {
                    Value = r.ID.ToString(),
                    Text = r.Description
                });

                ViewBag.Roles = userRoles;

                var exists = await UserService.Exists(model.Login, model.ID);

                if (exists)
                {
                    ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                }

                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    return View("Create", model);
                }

                var user = model.ToEntity();

                user = await UserService.UpdateAsync(user);

                if (user != null)
                {
                    return RedirectToAction("List", "User");
                }

                ModelState.AddModelError("", "Не удалось обновить пользователя.");
                ViewData["IsEdit"] = true;
                return View("Create", model);
            }, ex =>
            {
                ModelState.AddModelError("", ex);
                ViewData["IsEdit"] = true;
                return View("Create", model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var roles = await _roleService.GetAllAsync();

                var userRoles = roles.Select(r => new SelectListItem
                {
                    Value = r.ID.ToString(),
                    Text = r.Description
                });

                ViewBag.Roles = userRoles;

                var exists = await UserService.Exists(model.Login);

                if (exists)
                {
                    ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = model.ToEntity();

                user = await UserService.CreateAsync(user);

                if (user != null)
                {
                    return RedirectToAction("List", "User");
                }

                ModelState.AddModelError("Login", "Пользователь с таким логином уже существует в системе");
                return View(model);
            }, ex =>
            {
                ModelState.AddModelError("", ex);
                return View(model);
            });
        }

        #endregion                     
    }
}