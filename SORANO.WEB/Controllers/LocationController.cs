using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Dtos;
using SORANO.BLL.Services;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels;
using SORANO.WEB.ViewModels.Attachment;
using SORANO.WEB.ViewModels.Location;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable Mvc.ViewNotResolved

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    [CheckUserFilter]
    public class LocationController : EntityBaseController<LocationCreateUpdateViewModel>
    {
        private readonly ILocationService _locationService;
        private readonly IMapper _mapper;

        public LocationController(ILocationService locationService, 
            IUserService userService,
            IHostingEnvironment environment,
            ILocationTypeService locationTypeService, 
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache, 
            IMapper mapper) : base(userService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _locationService = locationService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await TryGetActionResultAsync(async () =>
            {
                var showDeletedLocations = Session.GetBool("ShowDeletedLocations");
                var showDeletedLocationTypes = Session.GetBool("ShowDeletedLocationTypes");

                var locationsResult = await _locationService.GetAllAsync(showDeletedLocations);

                if (locationsResult.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить список мест.";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ShowDeletedLocations = showDeletedLocations;
                ViewBag.ShowDeletedLocationTypes = showDeletedLocationTypes;

                await ClearAttachments();

                return View(_mapper.Map<IEnumerable<LocationIndexViewModel>>(locationsResult.Result));
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Home");
            });          
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
            return await TryGetActionResultAsync(async () =>
            {
                LocationCreateUpdateViewModel model;

                if (TryGetCached(out var cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
                {
                    model = cachedForSelectMainPicture;
                    await CopyMainPicture(model);
                }
                else if (TryGetCached(out var cachedForCreateType, CacheKeys.CreateLocationTypeCacheKey, CacheKeys.CreateLocationTypeCacheValidKey))
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

                return View(model);
            }, OnFault);            
        }       

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                LocationCreateUpdateViewModel model;

                if (TryGetCached(out var cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedForSelectMainPicture.ID == id)
                {
                    model = cachedForSelectMainPicture;
                    await CopyMainPicture(model);
                }
                else if (TryGetCached(out var cachedForCreateType, CacheKeys.CreateLocationTypeCacheKey, CacheKeys.CreateLocationTypeCacheValidKey) && cachedForCreateType.ID == id)
                {
                    model = cachedForCreateType;
                }
                else
                {
                    var result = await _locationService.GetAsync(id);

                    if (result.Status != ServiceResponseStatus.Success)
                    {
                        TempData["Error"] = "Не удалось найти указанное место.";
                        return RedirectToAction("Index");
                    }

                    model = _mapper.Map<LocationCreateUpdateViewModel>(result.Result);
                    model.IsUpdate = true;
                }

                return View("Create", model);
            }, OnFault);          
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _locationService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанное место.";
                    return RedirectToAction("Index");
                }

                return View(_mapper.Map<LocationDetailsViewModel>(result.Result));
            }, OnFault);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _locationService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанное место.";
                    return RedirectToAction("Index");
                }

                return View(_mapper.Map<LocationDeleteViewModel>(result.Result));
            }, OnFault);
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachmentsFilter]
        [ValidateModelFilter]
        public async Task<IActionResult> Create(LocationCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {            
                var location = _mapper.Map<LocationDto>(model);

                var result = await _locationService.CreateAsync(location, UserId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось создать место.";
                    return RedirectToAction("Index");
                }

                TempData["Success"] = $"Место \"{model.Name}\" было успешно создано.";

                if (string.IsNullOrEmpty(model.ReturnPath))
                {
                    return RedirectToAction("Index");
                }

                // TODO
                //if (MemoryCache.TryGetValue(CacheKeys.CreateLocationCacheKey, out DeliveryModel cachedDelivery))
                //{
                //    cachedDelivery.Location = location.ToModel();
                //    cachedDelivery.LocationID = location.ID.ToString();
                //    MemoryCache.Set(CacheKeys.CreateLocationCacheKey, cachedDelivery);
                //    Session.SetBool(CacheKeys.CreateLocationCacheValidKey, true);
                //}
                //else
                //{
                //    return BadRequest();
                //}

                return Redirect(model.ReturnPath);
            }, OnFault);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachmentsFilter]
        [ValidateModelFilter]
        public async Task<IActionResult> Update(LocationCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var location = _mapper.Map<LocationDto>(model);

                var result = await _locationService.UpdateAsync(location, UserId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось обновить место.";
                    return RedirectToAction("Index");
                }

                TempData["Success"] = $"Место \"{model.Name}\" было успешно обновлёно.";
                return RedirectToAction("Index");
            }, OnFault);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(LocationDeleteViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _locationService.DeleteAsync(model.ID, UserId);

                if (result.Status == ServiceResponseStatus.Success)
                {
                    TempData["Success"] = $"Место \"{model.Name}\" было успешноо помечено как удалённое.";
                }
                else
                {
                    TempData["Error"] = "Не удалось удалить место.";
                }

                return RedirectToAction("Index");
            }, OnFault);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachmentsFilter]
        public IActionResult CreateType(LocationCreateUpdateViewModel model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return TryGetActionResult(() =>
            {
                MemoryCache.Set(CacheKeys.CreateLocationTypeCacheKey, model);
                return RedirectToAction("Create", "LocationType", new { returnUrl });
            }, ex =>
            {
                TempData["Error"] = ex;
                return View("Create", model);
            });           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(LocationCreateUpdateViewModel model)
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

        [HttpPost]
        public async Task<JsonResult> GetLocations(string term)
        {
            var locations = await _locationService.GetAllAsync(false, term);

            return Json(new
            {
                results = locations.Result?
                    .Select(t => new
                    {
                        id = t.ID,
                        text = t.Name,
                        desc = t.Comment ?? string.Empty
                    })
                    .OrderBy(t => t.text)
            });
        }

        private IActionResult OnFault(string ex)
        {
            TempData["Error"] = ex;
            return RedirectToAction("Index");
        }
    }
}
