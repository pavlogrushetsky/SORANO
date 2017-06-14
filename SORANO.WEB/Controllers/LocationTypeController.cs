using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Models.LocationType;
using SORANO.WEB.Models.Recommendation;
using System.Threading.Tasks;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class LocationTypeController : BaseController
    {
        private readonly ILocationTypeService _locationTypeService;

        public LocationTypeController(ILocationTypeService locationTypeService, IUserService userService) : base(userService)
        {
            _locationTypeService = locationTypeService;
        }

        #region GET Actions

        [HttpGet]
        public IActionResult Create()
        {
            return View(new LocationTypeModel());
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LocationTypeModel model)
        {
            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                // Check the model
                if (!ModelState.IsValid)
                {
                    ModelState.RemoveDuplicateErrorMessages();
                    return View(model);
                }

                // Convert model to location type entity
                var locationType = model.ToEntity();

                // Get current user
                var currentUser = await GetCurrentUser();

                // Call correspondent service method to create new location type
                locationType = await _locationTypeService.CreateAsync(locationType, currentUser.ID);

                // If succeeded
                if (locationType != null)
                {
                    return RedirectToAction("Index", "Location");
                }

                // If failed
                ModelState.AddModelError("", "Не удалось создать новый тип мест.");
                return View(model);
            }, (ex) =>
            {
                ModelState.AddModelError("", ex);
                return View(model);
            });
        }

        [HttpPost]
        public IActionResult AddRecommendation(LocationTypeModel locationType, bool isEdit)
        {
            ModelState.Clear();

            locationType.Recommendations.Add(new RecommendationModel());

            ViewData["IsEdit"] = isEdit;

            return View("Create", locationType);
        }

        [HttpPost]
        public IActionResult DeleteRecommendation(LocationTypeModel locationType, bool isEdit, int num)
        {
            ModelState.Clear();

            locationType.Recommendations.RemoveAt(num);

            ViewData["IsEdit"] = isEdit;

            return View("Create", locationType);
        }

        #endregion
    }
}
