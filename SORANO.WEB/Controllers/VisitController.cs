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
        public PartialViewResult Create()
        {
            var model = new VisitCreateViewModel
            {
                Code = "мж2",
                Date = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                LocationID = LocationId ?? 0,
                LocationName = LocationName ?? string.Empty
            };

            return PartialView("_Visit", model);
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
                var result = await _visitService.CreateAsync(visit, UserId);
                if (result.Status != ServiceResponseStatus.Success)
                    return PartialView("_Visit", model);

                return Content(string.Empty);

            }, ex => Content(string.Empty));
        }
    }
}