using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Filters;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "administrator,manager")]
    [CheckUser]
    public class ReportController : BaseController
    {
        public ReportController(IUserService userService, IExceptionService exceptionService) : base(userService, exceptionService)
        {
        }

        public IActionResult Index() => View();
    }
}