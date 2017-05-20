using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Models.Role;
using SORANO.WEB.Models.User;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator")]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> Select()
        {
            var userCreateModel = TempData.Get<UserCreateModel>("UserCreateModel");

            if (userCreateModel == null)
            {
                return BadRequest();
            }

            var roles = await _roleService.GetAllAsync();

            var model = new RoleSelectModel
            {
                Roles = roles.Select(r => r.ToModel()).ToList(),
                User = userCreateModel
            };

            model.Roles.ToList().ForEach(r =>
            {
                r.IsSelected = userCreateModel.Roles?.Select(x => x.Name).Contains(r.Name) ?? false;
            });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Select(RoleSelectModel model)
        {
            var selectedRoles = model.Roles.Where(r => r.IsSelected);

            var userModel = model.User;

            userModel.Roles
                .Where(r => !selectedRoles.Contains(r))
                .ToList()
                .ForEach(r => userModel.Roles.Remove(r));

            selectedRoles
                .Where(r => !userModel.Roles.Contains(r))
                .ToList()
                .ForEach(r => userModel.Roles.Add(r));

            TempData.Put("UserCreateModel", userModel);

            return RedirectToAction("Create", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(RoleSelectModel model)
        {
            TempData.Put("UserCreateModel", model.User);

            return RedirectToAction("Create", "User");
        }
    }
}