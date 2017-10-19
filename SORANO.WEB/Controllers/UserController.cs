using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels;
using AutoMapper;
using SORANO.BLL.Services;
using SORANO.WEB.ViewModels.User;
using System.Linq;
using SORANO.BLL.Dtos;

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

                return View(new UserCreateUpdateViewModel { Roles = userRoles.ToList()});
            }, OnFault);           
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await UserService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанного пользователя.";
                    return RedirectToAction("Index");
                }

                var model = _mapper.Map<UserCreateUpdateViewModel>(result.Result);

                var roles = await _roleService.GetAllAsync();

                var userRoles = roles.Result.Select(r => new SelectListItem
                {
                    Value = r.ID.ToString(),
                    Text = r.Description,
                    Selected = result.Result.Roles.Select(x => x.ID).Contains(r.ID)
                });

                model.Roles = userRoles.ToList();
                model.IsUpdate = true;
                model.CanBeModified = id != UserId;

                return View("Create", model);
            }, OnFault);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await UserService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанного пользователя.";
                    return RedirectToAction("Index");
                }

                return View(_mapper.Map<UserDeleteViewModel>(result.Result));
            }, OnFault);           
        }

        [HttpGet]
        public async Task<IActionResult> Block(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await UserService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанного пользователя.";
                    return RedirectToAction("Index");
                }

                return View(_mapper.Map<UserBlockViewModel>(result.Result));
            }, OnFault);            
        }         

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await UserService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанного пользователя.";
                    return RedirectToAction("Index");
                }

                return View(_mapper.Map<UserDetailsViewModel>(result.Result));
            }, OnFault);           
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([Bind("ID")]UserModel model)
        {
            await UserService.DeleteAsync(model.ID);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block([Bind("ID")]UserModel model)
        {
            var user = await UserService.GetAsync(model.ID);

            user.Result.IsBlocked = !user.Result.IsBlocked;

            await UserService.UpdateAsync(user.Result);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserCreateUpdateViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return View("Create", model);
                }

                var user = _mapper.Map<UserDto>(model);

                var result = await UserService.UpdateAsync(user);

                switch (result.Status)
                {
                    case ServiceResponseStatus.NotFound:
                    case ServiceResponseStatus.InvalidOperation:
                        TempData["Error"] = "Не удалось обновить пользователя.";
                        return RedirectToAction("Index");
                    case ServiceResponseStatus.AlreadyExists:
                        ModelState.AddModelError("Login", "Пользователь с таким логином уже существует.");
                        return View("Create", model);
                }

                TempData["Success"] = $"Пользователь \"{model.Login}\" был успешно обновлён.";

                return RedirectToAction("Index");
            }, OnFault);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateUpdateViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = _mapper.Map<UserDto>(model);

                var result = await UserService.CreateAsync(user);

                switch (result.Status)
                {
                    case ServiceResponseStatus.NotFound:
                    case ServiceResponseStatus.InvalidOperation:
                        TempData["Error"] = "Не удалось создать пользователя.";
                        return RedirectToAction("Index");
                    case ServiceResponseStatus.AlreadyExists:
                        ModelState.AddModelError("Login", "Пользователь с таким логином уже существует.");
                        return View(model);
                }

                TempData["Success"] = $"Пользователь \"{model.Login}\" был успешно создан.";

                return RedirectToAction("Index");
            }, OnFault);
        }

        #endregion

        private IActionResult OnFault(string ex)
        {
            TempData["Error"] = ex;
            return RedirectToAction("Index");
        }
    }
}