using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.AccountEntities;
using SORANO.WEB.Models.User;
using SORANO.WEB.Infrastructure.Extensions;

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

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List view</returns>
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

        /// <summary>
        /// Delete specified user
        /// </summary>
        /// <param name="id">User identifier</param>
        /// <returns>Delete view</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetIncludeAllAsync(id);

            return View(user.ToDeleteModel(user.IsCurrent(HttpContext), user.HasActivities()));
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

            return View(user.ToBlockModel(user.IsCurrent(HttpContext)));
        }

        /// <summary>
        /// Update specified user
        /// </summary>
        /// <param name="id">User identifier</param>
        /// <returns>Update view</returns>
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var user = await _userService.GetIncludeRolesAsync(id);

            return View(user.ToUpdateModel(!user.IsCurrent(HttpContext)));
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <returns>Create view</returns>
        [HttpGet]
        public IActionResult Create()
        {
            var model = TempData.Get<UserCreateModel>("UserCreateModel") ?? new UserCreateModel();

            return View(model);
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

            var isCurrent = user.IsCurrent(HttpContext);
            var hasActivities = user.HasActivities();

            return View(user.ToDetailsModel(isCurrent, hasActivities));
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
        public async Task<IActionResult> Update([Bind("ID,Login,Description,NewPassword,RepeatPassword,Roles,CanBeModified")]UserUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userService.GetAsync(model.ID);

            var roles = await _roleService.GetAllAsync();

            user.FromUpdateModel(model, roles.ToList(), model.CanBeModified);

            await _userService.UpdateAsync(user);

            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Login,Description,Password,Roles")]UserCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var roles = await _roleService.GetAllAsync();

            var user = new User();
            user.FromCreateModel(model, roles.ToList());            

            user = await _userService.CreateAsync(user);

            if (user != null)
            {
                return RedirectToAction("List", "User");
            }

            ModelState.AddModelError("Login", "Пользователь с таким логином уже существует в системе");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectRoles([Bind("Login,Description,Roles")] UserCreateModel model)
        {
            TempData.Put("UserCreateModel", model);

            return RedirectToAction("Select", "Role");
        }

        #endregion                     
    }
}