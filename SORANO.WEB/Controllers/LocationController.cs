﻿using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using SORANO.WEB.Models.Location;
using Microsoft.Extensions.Caching.Memory;
using SORANO.WEB.Models.Attachment;
using Microsoft.AspNetCore.Http;
using MimeTypes;
using SORANO.WEB.Infrastructure;

// ReSharper disable Mvc.ViewNotResolved

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class LocationController : EntityBaseController<LocationModel>
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

            var locations = await _locationService.GetAllAsync(showDeletedLocations);

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
        public async Task<IActionResult> Create()
        {           
            LocationModel model;

            if (TryGetCached(out LocationModel cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
            {
                model = cachedForSelectMainPicture;
                await CopyMainPicture(model);
            }
            else if (TryGetCached(out LocationModel cachedForCreateType, CacheKeys.CreateLocationTypeCacheKey, CacheKeys.CreateLocationTypeCacheValidKey))
            {
                model = cachedForCreateType;
            }
            else
            {
                model = new LocationModel
                {
                    MainPicture = new AttachmentModel()
                };
            }

            ViewBag.AttachmentTypes = await GetAttachmentTypes();
            ViewBag.LocationTypes = await GetLocationTypes();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {            
            LocationModel model;

            if (TryGetCached(out LocationModel cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedForSelectMainPicture.ID == id)
            {
                model = cachedForSelectMainPicture;
                await CopyMainPicture(model);
            }
            else if (TryGetCached(out LocationModel cachedForCreateType, CacheKeys.CreateLocationTypeCacheKey, CacheKeys.CreateLocationTypeCacheValidKey) && cachedForCreateType.ID == id)
            {
                model = cachedForCreateType;
            }
            else
            {
                var location = await _locationService.GetAsync(id);

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
        public async Task<IActionResult> Create(LocationModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();
            var locationTypes = new List<SelectListItem>();

            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor("MainPicture");
                attachmentTypes = await GetAttachmentTypes();
                locationTypes = await GetLocationTypes();
                ViewBag.AttachmentTypes = attachmentTypes;
                ViewBag.LocationTypes = locationTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                for (var i = 0; i < model.Attachments.Count; i++)
                {
                    var extensions = model.Attachments[i]
                        .Type.MimeTypes?.Split(',')
                        .Select(MimeTypeMap.GetExtension);

                    if (extensions != null && !extensions.Contains(Path.GetExtension(model.Attachments[i].FullPath)))
                    {
                        ModelState.AddModelError($"Attachments[{i}].Name", "Вложение не соответствует указанному типу");
                    }
                }

                int.TryParse(model.TypeID, out int typeId);

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
                ModelState.RemoveFor("MainPicture");
                attachmentTypes = await GetAttachmentTypes();
                ViewBag.AttachmentTypes = attachmentTypes;
                ViewBag.LocationTypes = await GetLocationTypes();

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                for (var i = 0; i < model.Attachments.Count; i++)
                {
                    var extensions = model.Attachments[i]
                        .Type.MimeTypes?.Split(',')
                        .Select(MimeTypeMap.GetExtension);

                    if (extensions != null && !extensions.Contains(Path.GetExtension(model.Attachments[i].FullPath)))
                    {
                        ModelState.AddModelError($"Attachments[{i}].Name", "Вложение не соответствует указанному типу");
                    }
                }

                int.TryParse(model.TypeID, out int typeId);

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
                var currentUser = await GetCurrentUser();

                await _locationService.DeleteAsync(model.ID, currentUser.ID);

                return RedirectToAction("Index", "Location");
            }, ex => RedirectToAction("Index", "Location"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateType(LocationModel model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            await LoadMainPicture(model, mainPictureFile);
            await LoadAttachments(model, attachments);

            _memoryCache.Set(CacheKeys.CreateLocationTypeCacheKey, model);

            return RedirectToAction("Create", "LocationType", new { returnUrl });
        }

        #endregion

        private async Task<List<SelectListItem>> GetLocationTypes()
        {
            var types = await _locationTypeService.GetAllAsync(false);

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

            _memoryCache.Set(CacheKeys.LocationTypesCacheKey, locationTypes);

            return locationTypes;
        }
    }
}
