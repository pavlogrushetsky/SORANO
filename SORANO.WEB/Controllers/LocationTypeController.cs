using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeTypes;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Models;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class LocationTypeController : EntityBaseController<LocationTypeModel>
    {
        private readonly ILocationTypeService _locationTypeService;

        public LocationTypeController(ILocationTypeService locationTypeService, 
            IUserService userService,
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache) : base(userService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _locationTypeService = locationTypeService;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Create(string returnUrl)
        {
            await ClearAttachments();

            LocationTypeModel model;

            if (TryGetCached(out LocationTypeModel cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
            {
                model = cachedForSelectMainPicture;
                await CopyMainPicture(model);
            }
            else
            {
                model = new LocationTypeModel
                {
                    MainPicture = new AttachmentModel(),
                    ReturnPath = returnUrl
                };
            }

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            return View(model);
        }

        [HttpGet]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedLocationTypes", show);

            return RedirectToAction("Index", "Location");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            await ClearAttachments();

            LocationTypeModel model;

            if (TryGetCached(out LocationTypeModel cachedModel, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedModel.ID == id)
            {
                model = cachedModel;
                await CopyMainPicture(model);
            }
            else
            {
                var locationType = await _locationTypeService.GetAsync(id);

                model = locationType.ToModel();
            }

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = true;

            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            await ClearAttachments();

            var locationType = await _locationTypeService.GetAsync(id);

            return View(locationType.ToModel());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            await ClearAttachments();

            var locationType = await _locationTypeService.GetIncludeAllAsync(id);

            return View(locationType.ToModel());
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LocationTypeModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();

            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor("MainPicture");
                attachmentTypes = await GetAttachmentTypes();
                ViewBag.AttachmentTypes = attachmentTypes;

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
                    if (string.IsNullOrEmpty(model.ReturnPath))
                    {
                        return RedirectToAction("Index", "Location");
                    }

                    if (_memoryCache.TryGetValue(CacheKeys.CreateLocationTypeCacheKey, out LocationModel cachedLocation))
                    {
                        cachedLocation.Type = locationType.ToModel();
                        cachedLocation.TypeID = locationType.ID.ToString();
                        _memoryCache.Set(CacheKeys.CreateLocationTypeCacheKey, cachedLocation);
                        Session.SetBool(CacheKeys.CreateLocationTypeCacheValidKey, true);
                    }
                    else
                    {
                        return BadRequest();
                    }

                    return Redirect(model.ReturnPath);
                }

                // If failed
                ModelState.AddModelError("", "Не удалось создать новый тип мест.");
                return View(model);
            }, ex =>
            {
                ViewBag.AttachmentTypes = attachmentTypes;
                ModelState.AddModelError("", ex);
                return View(model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(LocationTypeModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();

            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor("MainPicture");
                attachmentTypes = await GetAttachmentTypes();
                ViewBag.AttachmentTypes = attachmentTypes;

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

                // Check the model
                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    ModelState.RemoveDuplicateErrorMessages();
                    return View("Create", model);
                }

                // Convert model to location type entity
                var locationType = model.ToEntity();

                // Get current user
                var currentUser = await GetCurrentUser();

                // Call correspondent service method to update location type
                locationType = await _locationTypeService.UpdateAsync(locationType, currentUser.ID);

                // If succeeded
                if (locationType != null)
                {
                    return RedirectToAction("Index", "Location");
                }

                // If failed
                ModelState.AddModelError("", "Не удалось обновить тип мест.");
                ViewData["IsEdit"] = true;
                return View("Create", model);
            }, ex =>
            {
                ViewBag.AttachmentTypes = attachmentTypes;
                ModelState.AddModelError("", ex);
                ViewData["IsEdit"] = true;
                return View("Create", model);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(LocationTypeModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var currentUser = await GetCurrentUser();

                await _locationTypeService.DeleteAsync(model.ID, currentUser.ID);

                return RedirectToAction("Index", "Location");
            }, ex => RedirectToAction("Index", "Location"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(LocationTypeModel model)
        {
            if (string.IsNullOrEmpty(model.ReturnPath))
            {
                return RedirectToAction("Index", "Location");
            }

            if (_memoryCache.TryGetValue(CacheKeys.CreateLocationTypeCacheKey, out LocationModel _))
            {
                Session.SetBool(CacheKeys.CreateLocationTypeCacheValidKey, true);
            }

            return Redirect(model.ReturnPath);
        }

        #endregion
    }
}
