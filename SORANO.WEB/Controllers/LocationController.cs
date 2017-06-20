using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SORANO.WEB.Models.Location;
using SORANO.WEB.Models.Recommendation;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class LocationController : BaseController
    {
        private readonly ILocationService _locationService;
        private readonly ILocationTypeService _locationTypeService;

        public LocationController(ILocationService locationService, IUserService userService, ILocationTypeService locationTypeService) : base(userService)
        {
            _locationService = locationService;
            _locationTypeService = locationTypeService;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var locations = await _locationService.GetAllAsync();

            return View(locations.Select(l => l.ToModel()).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var types = await _locationTypeService.GetAllAsync();

            var locationTypes = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "0",
                    Text = "-- Тип места --"
                }
            };

            locationTypes.AddRange(types.Select(l => new SelectListItem
            {
                Value = l.ID.ToString(),
                Text = l.Name
            }));

            ViewBag.LocationTypes = locationTypes;

            return View(new LocationModel());
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LocationModel model)
        {
            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                var types = await _locationTypeService.GetAllAsync();

                var LocationTypes = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = "0",
                        Text = "-- Тип места --"
                    }
                };

                LocationTypes.AddRange(types.Select(l => new SelectListItem
                {
                    Value = l.ID.ToString(),
                    Text = l.Name
                }));

                ViewBag.LocationTypes = LocationTypes;

                int typeId;
                int.TryParse(model.Type, out typeId);

                if (typeId <= 0)
                {
                    ModelState.AddModelError("Type", "Необходимо указать тип места.");
                }

                // Check the model
                if (!ModelState.IsValid)
                {
                    ModelState.RemoveDuplicateErrorMessages();
                    return View(model);
                }                

                // Convert model to location entity
                var location = model.ToEntity();

                // Get current user
                var currentUser = await GetCurrentUser();

                // Call correspondent service method to create new location
                location = await _locationService.CreateAsync(location, currentUser.ID);

                // If succeeded
                if (location != null)
                {
                    return RedirectToAction("Index", "Location");
                }

                // If failed
                ModelState.AddModelError("", "Не удалось создать новый тип мест.");
                return View(model);
            }, ex =>
            {
                ModelState.AddModelError("", ex);
                return View(model);
            });
        }

        [HttpPost]
        public IActionResult AddRecommendation(LocationModel location, bool isEdit)
        {
            ModelState.Clear();

            location.Recommendations.Add(new RecommendationModel());

            ViewData["IsEdit"] = isEdit;

            return View("Create", location);
        }

        [HttpPost]
        public IActionResult DeleteRecommendation(LocationModel location, bool isEdit, int num)
        {
            ModelState.Clear();

            location.Recommendations.RemoveAt(num);

            ViewData["IsEdit"] = isEdit;

            return View("Create", location);
        }

        #endregion
    }
}
