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

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var location = await _locationService.GetAsync(id);

            var model = location.ToModel();

            var types = await _locationTypeService.GetAllAsync();

            var locationTypes = types.Select(t => new SelectListItem
            {
                Value = t.ID.ToString(),
                Text = t.Name,
                Selected = t.ID == location.TypeID
            });

            ViewBag.LocationTypes = locationTypes;

            ViewData["IsEdit"] = true;

            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var location = await _locationService.GetAsync(id);

            return View(location.ToModel());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var location = await _locationService.GetAsync(id);

            return View(location.ToModel());
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
                int.TryParse(model.TypeID, out typeId);

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
                ModelState.AddModelError("", "Не удалось создать новое место.");
                return View(model);
            }, ex =>
            {
                ModelState.AddModelError("", ex);
                return View(model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(LocationModel model)
        {
            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                var types = await _locationTypeService.GetAllAsync();

                var locationTypes = types.Select(t => new SelectListItem
                {
                    Value = t.ID.ToString(),
                    Text = t.Name
                });

                ViewBag.LocationTypes = locationTypes;

                int typeId;
                int.TryParse(model.TypeID, out typeId);

                if (typeId <= 0)
                {
                    ModelState.AddModelError("Type", "Необходимо указать тип места.");
                }

                // Check the model
                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    ModelState.RemoveDuplicateErrorMessages();
                    return View(model);
                }

                // Convert model to location entity
                var location = model.ToEntity();

                // Get current user
                var currentUser = await GetCurrentUser();

                // Call correspondent service method to update location
                location = await _locationService.UpdateAsync(location, currentUser.ID);

                // If succeeded
                if (location != null)
                {
                    return RedirectToAction("Index", "Location");
                }

                // If failed
                ModelState.AddModelError("", "Не удалось обновить место.");
                ViewData["IsEdit"] = true;
                return View(model);
            }, ex =>
            {
                ModelState.AddModelError("", ex);
                ViewData["IsEdit"] = true;
                return View(model);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(LocationModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var currentUser = await GetCurrentUser();

                await _locationService.DeleteAsync(model.ID, currentUser.ID);

                return RedirectToAction("Index", "Location");
            }, ex => RedirectToAction("Index", "Location"));
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
