using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels;
using AutoMapper;
using SORANO.BLL.Services;
using SORANO.WEB.ViewModels.User;
using System.Linq;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer, administrator")]
    [CheckUserFilter]
    public class UserController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, 
            IRoleService roleService,
            IMapper mapper) : base(userService)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await TryGetActionResultAsync(async () => 
            {
                var usersResult = await UserService.GetAllAsync();

                if (usersResult.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить список пользователей.";
                    return RedirectToAction("Index", "Home");
                }

                return View(_mapper.Map<IEnumerable<UserIndexViewModel>>(usersResult.Result));
            }, ex => 
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Home");
            });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return await TryGetActionResultAsync(async () => 
            {
                var roles = await _roleService.GetAllAsync();

                var userRoles = roles.Result.Select(r => new SelectListItem
                {
                    Value = r.ID.ToString(),
                    Text = r.Description
                });

                ViewBag.Roles = userRoles;

                return View(new UserModel());
            }, ex => 
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index");
            });           
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await UserService.GetAsync(id);

            return View(_mapper.Map<UserDeleteViewModel>(user.Result));
        }

        [HttpGet]
        public async Task<IActionResult> Block(int id)
        {
            var user = await UserService.GetAsync(id);

            return View(_mapper.Map<UserBlockViewModel>(user.Result));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {           
            var user = await UserService.GetAsync(id);

            var model = _mapper.Map<UserCreateUpdateViewModel>(user.Result);

            var roles = await _roleService.GetAllAsync();

            var userRoles = roles.Result.Select(r => new SelectListItem
            {
                Value = r.ID.ToString(),
                Text = r.Description,
                Selected = user.Result.Roles.Select(x => x.ID).Contains(r.ID)
            });

            ViewBag.Roles = userRoles;

            ViewData["IsEdit"] = true;

            return View("Create", model);
        }   

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await UserService.GetAsync(id);

            return View(_mapper.Map<UserDetailsViewModel>(user.Result));
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

            user.Result.IsBlocked = !user.Result.IsBlocked;

            await UserService.UpdateAsync(user.Result);

            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var roles = await _roleService.GetAllAsync();

                var userRoles = roles.Result.Select(r => new SelectListItem
                {
                    Value = r.ID.ToString(),
                    Text = r.Description
                });

                ViewBag.Roles = userRoles;

                var exists = await UserService.Exists(model.Login, model.ID);

                if (exists.Result)
                {
                    ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                }

                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    return View("Create", model);
                }

                var user = model.ToEntity();

                // TODO
                //user = await UserService.UpdateAsync(user);

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

                var userRoles = roles.Result.Select(r => new SelectListItem
                {
                    Value = r.ID.ToString(),
                    Text = r.Description
                });

                ViewBag.Roles = userRoles;

                var exists = await UserService.Exists(model.Login);

                if (exists.Result)
                {
                    ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = model.ToEntity();

                // TODO
                //user = await UserService.CreateAsync(user);

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