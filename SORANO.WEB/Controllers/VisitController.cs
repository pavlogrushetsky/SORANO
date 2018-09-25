using System;
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

        [HttpGet]
        public PartialViewResult Create()
        {
            var model = new VisitCreateViewModel
            {
                Code = "мж2",
                Date = DateTime.Now.ToString("dd.MM.yyyy"),
                LocationID = LocationId ?? 0,
                LocationName = LocationName ?? string.Empty
            };

            return PartialView("_Visit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(VisitCreateViewModel model)
        {
            return TryGetActionResult(() =>
            {
                if (ModelState.IsValid)
                    return Content(string.Empty);

                return PartialView("_Visit", model);

            }, OnFault);
        }

        private IActionResult OnFault(string ex)
        {
            TempData["Error"] = ex;
            return RedirectToAction("Index", "Home");
        }
    }
}