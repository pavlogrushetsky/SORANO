using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Models.User;
using SORANO.WEB.Infrastructure.Extensions;

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

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List view</returns>
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = await _userService.GetAllIncludeAllAsync();

            var models = new List<UserModel>();

            var currentUser = await GetCurrentUser();

            users.ForEach(u =>
            {
                models.Add(u.ToModel(u.ID == currentUser.ID));
            });

            return View(models);
        }

        /// <summary>
        /// Delete specified user
        /// </summary>
        /// <param name="id">User identifier</param>
        /// <returns>Delete view</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetIncludeAllAsync(id);

            var currentUser = await GetCurrentUser();

            return View(user.ToModel(user.ID == currentUser.ID));
        }

        /// <summary>
        /// Block specified user
        /// </summary>
        /// <param name="id">User identifier</param>
        /// <returns>Block view</returns>
        [HttpGet]
        public async Task<IActionResult> Block(int id)
        {
            var user = await _userService.GetAsync(id);

            var currentUser = GetCurrentUser();

            return View(user.ToModel(user.ID == currentUser.Id));
        }

        /// <summary>
        /// Update specified user
        /// </summary>
        /// <param name="id">User identifier</param>
        /// <returns>Update view</returns>
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {           
            var user = await _userService.GetAsync(id);

            var currentUser = await GetCurrentUser();

            var model = user.ToModel(user.ID == currentUser.ID);

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

        /// <summary>
        /// Create new user
        /// </summary>
        /// <returns>Create view</returns>
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

        /// <summary>
        /// Get user details
        /// </summary>
        /// <param name="id">User identifier</param>
        /// <returns>Details view</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetIncludeAllAsync(id);

            var currentUser = await GetCurrentUser();

            return View(user.ToModel(user.ID == currentUser.ID));
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([Bind("ID")]UserModel model)
        {
            await _userService.DeleteAsync(model.ID);

            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block([Bind("ID")]UserModel model)
        {
            var user = await _userService.GetAsync(model.ID);

            user.IsBlocked = !user.IsBlocked;

            await _userService.UpdateAsync(user);

            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    return View("Create", model);
                }

                var user = model.ToEntity();

                user = await _userService.UpdateAsync(user);

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

                if (model.RoleIDs == null || !model.RoleIDs.Any())
                {
                    ModelState.AddModelError("RoleIDs", "Пользователю необходимо назначить хотя бы одну роль.");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = model.ToEntity();

                user = await _userService.CreateAsync(user);

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