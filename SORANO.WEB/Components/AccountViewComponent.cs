using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Dtos;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.ViewModels.Account;

namespace SORANO.WEB.Components
{
    public class AccountViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AccountViewComponent(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var login = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            var location = HttpContext.User.FindFirst("LocationName")?.Value;

            var userResult = await _userService.GetAsync(login);

            var model = _mapper.Map<UserDto, AccountViewModel>(userResult.Result);
            model.LocationName = location;
            model.IsEditor = userResult.Result.Roles.Any(r => r.Name.Equals("editor"));

            return View(model);
        }
    }
}