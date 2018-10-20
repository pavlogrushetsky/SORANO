using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Dtos;
using SORANO.BLL.Services;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels.Visit;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager,editor,user")]
    [CheckUser]
    public class VisitController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IVisitService _visitService;

        public VisitController(IUserService userService, 
            IExceptionService exceptionService, 
            IMapper mapper, 
            IVisitService visitService) 
            : base(userService, exceptionService)
        {
            _mapper = mapper;
            _visitService = visitService;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Table() => ViewComponent("VisitsTable");

        [HttpGet]
        public PartialViewResult Create()
        {
            return PartialView("_Visit", new VisitCreateViewModel
            {
                Code = "мж2",
                Date = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                LocationID = LocationId ?? 0,
                LocationName = LocationName ?? string.Empty
            });
        }

        [HttpGet]
        public async Task<PartialViewResult> Update(int id)
        {
            var model = await _visitService.GetAsync(id);
            return PartialView("_Visit", new VisitCreateViewModel
            {
                ID = model.Result.ID,
                Code = model.Result.Code,
                Date = model.Result.Date.ToString("dd.MM.yyyy HH:mm"),
                LocationID = model.Result.LocationID,
                LocationName = model.Result.LocationName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitCreateViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                if (!ModelState.IsValid)
                    return PartialView("_Visit", model);

                var visit = _mapper.Map<VisitDto>(model);
                bool success;
                if (model.ID == 0)
                {
                    var result = await _visitService.CreateAsync(visit, UserId);
                    success = result.Status == ServiceResponseStatus.Success;
                }
                else
                {
                    var result = await _visitService.UpdateAsync(visit, UserId);
                    success = result.Status == ServiceResponseStatus.Success;
                }

                if (!success)
                    return PartialView("_Visit", model);

                return Content(string.Empty);

            }, ex => Content(string.Empty));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _visitService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанный артикул.";
                    return RedirectToAction("Index");
                }

                var model = new VisitCreateViewModel
                {
                    ID = result.Result.ID,
                    Code = result.Result.Code,
                    Date = result.Result.Date.ToString("dd.MM.yyyy HH:mm"),
                    LocationID = result.Result.LocationID,
                    LocationName = result.Result.LocationName
                };
                return View(model);
            }, OnFault);
        }

        [HttpPost]
        [Authorize(Roles = "developer,administrator,manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(VisitCreateViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _visitService.DeleteAsync(model.ID, UserId);

                if (result.Status == ServiceResponseStatus.Success)
                {
                    TempData["Success"] = "Посещение было успешно удалено.";
                }
                else
                {
                    TempData["Error"] = "Не удалось удалить посещение.";
                }

                return RedirectToAction("Index");
            }, OnFault);
        }

        private IActionResult OnFault(string ex)
        {
            TempData["Error"] = ex;
            return RedirectToAction("Index");
        }
    }
}