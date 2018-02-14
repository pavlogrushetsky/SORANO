using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.ViewModels.LocationType;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.WEB.Components
{
    public class LocationTypesViewComponent : ViewComponent
    {
        private readonly ILocationTypeService _locationTypeService;
        private readonly IMapper _mapper;

        public LocationTypesViewComponent(ILocationTypeService locationTypeService, IMapper mapper)
        {
            _locationTypeService = locationTypeService;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool withDeleted = false)
        {
            var locationTypes = await _locationTypeService.GetAllAsync(withDeleted);

            return View(_mapper.Map<IEnumerable<LocationTypeIndexViewModel>>(locationTypes.Result));
        }
    }
}
