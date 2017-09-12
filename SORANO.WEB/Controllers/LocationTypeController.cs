using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels;
using SORANO.WEB.ViewModels.Attachment;
using SORANO.WEB.ViewModels.LocationType;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    [CheckUserFilter]
    public class LocationTypeController : EntityBaseController<LocationTypeCreateUpdateViewModel>
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

            LocationTypeCreateUpdateViewModel model;

            if (TryGetCached(out LocationTypeCreateUpdateViewModel cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
            {
                model = cachedForSelectMainPicture;
                await CopyMainPicture(model);
            }
            else
            {
                model = new LocationTypeCreateUpdateViewModel
                {
                    MainPicture = new MainPictureViewModel(),
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

            LocationTypeCreateUpdateViewModel model;

            if (TryGetCached(out LocationTypeCreateUpdateViewModel cachedModel, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedModel.ID == id)
            {
                model = cachedModel;
                await CopyMainPicture(model);
            }
            else
            {
                var locationType = await _locationTypeService.GetAsync(id, UserId);

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

            var locationType = await _locationTypeService.GetAsync(id, UserId);

            return View(locationType.ToModel());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            await ClearAttachments();

            var locationType = await _locationTypeService.GetAsync(id, UserId);

            return View(locationType.ToModel());
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LocationTypeCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();

            return await TryGetActionResultAsync(async () =>
            {
                attachmentTypes = await GetAttachmentTypes();
                ViewBag.AttachmentTypes = attachmentTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                var typeExists = await _locationTypeService.Exists(model.Name);

                if (typeExists)
                {
                    ModelState.AddModelError("Name", "Тип места с таким названием уже существует");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var locationType = model.ToEntity();

                locationType = await _locationTypeService.CreateAsync(locationType, UserId);

                if (locationType != null)
                {
                    if (string.IsNullOrEmpty(model.ReturnPath))
                    {
                        return RedirectToAction("Index", "Location");
                    }

                    if (MemoryCache.TryGetValue(CacheKeys.CreateLocationTypeCacheKey, out LocationModel cachedLocation))
                    {
                        cachedLocation.Type = locationType.ToModel();
                        cachedLocation.TypeID = locationType.ID.ToString();
                        MemoryCache.Set(CacheKeys.CreateLocationTypeCacheKey, cachedLocation);
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
        public async Task<IActionResult> Update(LocationTypeCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();

            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                attachmentTypes = await GetAttachmentTypes();
                ViewBag.AttachmentTypes = attachmentTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                var typeExists = await _locationTypeService.Exists(model.Name, model.ID);

                if (typeExists)
                {
                    ModelState.AddModelError("Name", "Тип места с таким названием уже существует");
                }

                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    return View("Create", model);
                }

                var locationType = model.ToEntity();

                locationType = await _locationTypeService.UpdateAsync(locationType, UserId);

                if (locationType != null)
                {
                    return RedirectToAction("Index", "Location");
                }

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
                await _locationTypeService.DeleteAsync(model.ID, UserId);

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

            if (MemoryCache.TryGetValue(CacheKeys.CreateLocationTypeCacheKey, out LocationModel _))
            {
                Session.SetBool(CacheKeys.CreateLocationTypeCacheValidKey, true);
            }

            return Redirect(model.ReturnPath);
        }

        #endregion
    }
}
