using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.ViewModels;
using SORANO.WEB.ViewModels.Attachment;
using SORANO.WEB.ViewModels.Common;
using SORANO.WEB.ViewModels.Location;

// ReSharper disable Mvc.ViewNotResolved

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class LocationController : EntityBaseController<LocationCreateUpdateViewModel>
    {
        private readonly ILocationService _locationService;
        private readonly ILocationTypeService _locationTypeService;

        public LocationController(ILocationService locationService, 
            IUserService userService,
            IHostingEnvironment environment,
            ILocationTypeService locationTypeService, 
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache) : base(userService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _locationService = locationService;
            _locationTypeService = locationTypeService;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var showDeletedLocations = Session.GetBool("ShowDeletedLocations");
            var showDeletedLocationTypes = Session.GetBool("ShowDeletedLocationTypes");

            var locations = await _locationService.GetAllAsync(showDeletedLocations, UserId);

            ViewBag.ShowDeletedLocations = showDeletedLocations;
            ViewBag.ShowDeletedLocationTypes = showDeletedLocationTypes;

            await ClearAttachments();

            return View(locations.Select(l => l.ToModel()).ToList());
        }

        [HttpGet]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedLocations", show);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create(string returnUrl)
        {           
            LocationCreateUpdateViewModel model;

            if (TryGetCached(out LocationCreateUpdateViewModel cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
            {
                model = cachedForSelectMainPicture;
                await CopyMainPicture(model);
            }
            else if (TryGetCached(out LocationCreateUpdateViewModel cachedForCreateType, CacheKeys.CreateLocationTypeCacheKey, CacheKeys.CreateLocationTypeCacheValidKey))
            {
                model = cachedForCreateType;
            }
            else
            {
                model = new LocationCreateUpdateViewModel
                {
                    MainPicture = new MainPictureViewModel(),
                    ReturnPath = returnUrl
                };
            }

            ViewBag.AttachmentTypes = await GetAttachmentTypes();
            ViewBag.LocationTypes = await GetLocationTypes();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {            
            LocationCreateUpdateViewModel model;

            if (TryGetCached(out LocationCreateUpdateViewModel cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedForSelectMainPicture.ID == id)
            {
                model = cachedForSelectMainPicture;
                await CopyMainPicture(model);
            }
            else if (TryGetCached(out LocationCreateUpdateViewModel cachedForCreateType, CacheKeys.CreateLocationTypeCacheKey, CacheKeys.CreateLocationTypeCacheValidKey) && cachedForCreateType.ID == id)
            {
                model = cachedForCreateType;
            }
            else
            {
                var location = await _locationService.GetAsync(id, UserId);

                model = location.ToModel();
            }

            ViewBag.AttachmentTypes = await GetAttachmentTypes();
            ViewBag.LocationTypes = await GetLocationTypes();

            ViewData["IsEdit"] = true;

            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var location = await _locationService.GetAsync(id, UserId);

            return View(location.ToModel());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var location = await _locationService.GetAsync(id, UserId);

            return View(location.ToModel());
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LocationModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();
            var locationTypes = new List<SelectListItem>();

            return await TryGetActionResultAsync(async () =>
            {
                attachmentTypes = await GetAttachmentTypes();
                locationTypes = await GetLocationTypes();

                ViewBag.AttachmentTypes = attachmentTypes;
                ViewBag.LocationTypes = locationTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                if (!ModelState.IsValid)
                {
                    return View(model);
                }                

                var location = model.ToEntity();

                location = await _locationService.CreateAsync(location, UserId);

                if (location != null)
                {
                    if (string.IsNullOrEmpty(model.ReturnPath))
                    {
                        return RedirectToAction("Index", "Location");
                    }

                    if (MemoryCache.TryGetValue(CacheKeys.CreateLocationCacheKey, out DeliveryModel cachedDelivery))
                    {
                        cachedDelivery.Location = location.ToModel();
                        cachedDelivery.LocationID = location.ID.ToString();
                        MemoryCache.Set(CacheKeys.CreateLocationCacheKey, cachedDelivery);
                        Session.SetBool(CacheKeys.CreateLocationCacheValidKey, true);
                    }
                    else
                    {
                        return BadRequest();
                    }

                    return Redirect(model.ReturnPath);
                }

                // If failed
                ModelState.AddModelError("", "Не удалось создать новое место.");
                return View(model);
            }, ex =>
            {
                ViewBag.AttachmentTypes = attachmentTypes;
                ViewBag.LocationTypes = locationTypes;
                ModelState.AddModelError("", ex);
                return View(model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(LocationModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();
            var locationTypes = new List<SelectListItem>();

            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                attachmentTypes = await GetAttachmentTypes();
                ViewBag.AttachmentTypes = attachmentTypes;
                ViewBag.LocationTypes = await GetLocationTypes();

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    return View(model);
                }

                var location = model.ToEntity();

                location = await _locationService.UpdateAsync(location, UserId);

                if (location != null)
                {
                    return RedirectToAction("Index", "Location");
                }

                ModelState.AddModelError("", "Не удалось обновить место.");
                ViewData["IsEdit"] = true;
                return View(model);
            }, ex =>
            {
                ViewBag.AttachmentTypes = attachmentTypes;
                ViewBag.LocationTypes = locationTypes;
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
                await _locationService.DeleteAsync(model.ID, UserId);

                return RedirectToAction("Index", "Location");
            }, ex => RedirectToAction("Index", "Location"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateType(LocationModel model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            await LoadMainPicture(model, mainPictureFile);
            await LoadAttachments(model, attachments);

            MemoryCache.Set(CacheKeys.CreateLocationTypeCacheKey, model);

            return RedirectToAction("Create", "LocationType", new { returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(LocationModel model)
        {
            if (string.IsNullOrEmpty(model.ReturnPath))
            {
                return RedirectToAction("Index", "Location");
            }

            if (MemoryCache.TryGetValue(CacheKeys.CreateLocationCacheKey, out DeliveryModel _))
            {
                Session.SetBool(CacheKeys.CreateLocationCacheValidKey, true);
            }

            return Redirect(model.ReturnPath);
        }

        #endregion

        private async Task<List<SelectListItem>> GetLocationTypes()
        {
            var types = await _locationTypeService.GetAllAsync(false, UserId);

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

            MemoryCache.Set(CacheKeys.LocationTypesCacheKey, locationTypes);

            return locationTypes;
        }
    }
}
