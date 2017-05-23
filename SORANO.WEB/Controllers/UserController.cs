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

            var models = new List<UserModel>();

            users.ForEach(u => 
            {
                var isCurrent = u.IsCurrent(HttpContext);
                models.Add(u.ToModel(isCurrent));
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

            return View(user.ToModel(user.IsCurrent(HttpContext)));
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

            return View(user.ToModel(user.IsCurrent(HttpContext)));
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

            var model = TempData.Get<UserModel>("UserModel") ?? user.ToModel(user.IsCurrent(HttpContext));

            return View(model);
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <returns>Create view</returns>
        [HttpGet]
        public IActionResult Create()
        {
            var model = TempData.Get<UserModel>("UserModel") ?? new UserModel();

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

            return View(user.ToModel(user.IsCurrent(HttpContext)));
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
        public async Task<IActionResult> Create(UserModel model)
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
        public IActionResult SelectRoles([Bind("ID,CanBeModified,Login,Description,Roles")] UserModel model, string returnUrl)
        {
            TempData.Put("UserModel", model);

            return RedirectToAction("Select", "Role", new { returnUrl });
        }

        #endregion                     
    }
}