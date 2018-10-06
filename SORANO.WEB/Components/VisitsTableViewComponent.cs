using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.ViewModels.Visit;

namespace SORANO.WEB.Components
{
    public class VisitsTableViewComponent : ViewComponent
    {
        private readonly IVisitService _visitService;
        private readonly IMapper _mapper;

        public VisitsTableViewComponent(IVisitService visitService, IMapper mapper)
        {
            _visitService = visitService;
            _mapper = mapper;
        }

#pragma warning disable 1998
        public async Task<IViewComponentResult> InvokeAsync()
#pragma warning restore 1998
        {
            var visitsResult = _visitService.GetAll(true);
            if (visitsResult.Status != ServiceResponseStatus.Success)
            {
                
            }

            var viewModel = _mapper.Map<VisitTableViewModel>(visitsResult.Result);

            return View(viewModel);
        }
    }
}