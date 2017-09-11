using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using System.Threading.Tasks;

namespace SORANO.WEB.Components
{
    public class LocationTypesViewComponent : ViewComponent
    {
        private readonly ILocationTypeService _locationTypeService;

        public LocationTypesViewComponent(ILocationTypeService locationTypeService)
        {
            _locationTypeService = locationTypeService;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool withDeleted = false)
        {
            var locationTypes = await _locationTypeService.GetAllAsync(withDeleted);

            return View(locationTypes.Select(t => t.ToModel()).ToList());
        }
    }
}
