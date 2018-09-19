using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels.Visit;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager,editor,user")]
    [CheckUser]
    public class VisitController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IExceptionService _exceptionService;

        public VisitController(IUserService userService, 
            IExceptionService exceptionService) 
            : base(userService, exceptionService)
        {
            _userService = userService;
            _exceptionService = exceptionService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(VisitCreateViewModel model)
        {
            return TryGetActionResult(() =>
            {
                return NoContent();

            }, OnFault);
        }

        private IActionResult OnFault(string ex)
        {
            TempData["Error"] = ex;
            return NoContent();
        }
    }
}