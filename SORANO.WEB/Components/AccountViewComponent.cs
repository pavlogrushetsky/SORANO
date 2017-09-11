using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

            var userResult = await _userService.GetAsync(login);

            return View(_mapper.Map<AccountViewModel>(userResult.Result));
        }
    }
}