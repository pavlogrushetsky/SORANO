using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
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
            if (!TempData.ContainsKey("UserCreateModel"))
            {
                return BadRequest();
            }

            var userCreateModel = JsonConvert.DeserializeObject<UserCreateModel>(TempData["UserCreateModel"].ToString());

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

            return View(model);
        }
    }
}