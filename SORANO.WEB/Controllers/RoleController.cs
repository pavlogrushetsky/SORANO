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
        public async Task<IActionResult> Select(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                return BadRequest();
            }

            var userModel = TempData.Get<UserModel>("UserModel");

            if (userModel == null)
            {
                return Redirect(returnUrl);
            }

            var roles = await _roleService.GetAllAsync();

            var model = new RoleSelectModel
            {
                Roles = roles.Select(r => r.ToModel()).ToList(),
                User = userModel,
                ReturnUrl = returnUrl
            };

            model.Roles.ToList().ForEach(r =>
            {
                r.IsSelected = userModel.Roles?.Select(x => x.Name).Contains(r.Name) ?? false;
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

            TempData.Put("UserModel", userModel);

            return Redirect(model.ReturnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(RoleSelectModel model)
        {
            TempData.Put("UserModel", model.User);

            return Redirect(model.ReturnUrl);
        }
    }
}