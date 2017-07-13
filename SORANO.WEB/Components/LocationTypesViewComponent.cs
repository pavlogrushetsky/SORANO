using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace SORANO.WEB.Components
{
    /// <summary>
    /// View component for rendering location types table view
    /// </summary>
    public class LocationTypesViewComponent : ViewComponent
    {
        private readonly ILocationTypeService _locationTypeService;

        /// <summary>
        /// View component for rendering location types table view
        /// </summary>
        /// <param name="locationTypeService">Location types service</param>
        public LocationTypesViewComponent(ILocationTypeService locationTypeService)
        {
            _locationTypeService = locationTypeService;
        }

        /// <summary>
        /// Invoke component asynchronously
        /// </summary>
        /// <param name="withDeleted">Show deleted location types</param>
        /// <returns>Component's default view</returns>
        public async Task<IViewComponentResult> InvokeAsync(bool withDeleted = false)
        {
            var locationTypes = await _locationTypeService.GetAllAsync(withDeleted);

            return View(locationTypes.Select(t => t.ToModel()).ToList());
        }
    }
}
