﻿using AutoMapper;
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
using SORANO.WEB.ViewModels.Attachment;
using SORANO.WEB.ViewModels.Location;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.WEB.ViewModels.Delivery;

// ReSharper disable Mvc.ViewNotResolved

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class LocationController : EntityBaseController<LocationCreateUpdateViewModel>
    {
        private readonly ILocationService _locationService;
        private readonly IMapper _mapper;

        public LocationController(ILocationService locationService, 
            IUserService userService,
            IExceptionService exceptionService,
            IHostingEnvironment environment,
            ILocationTypeService locationTypeService, 
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache, 
            IMapper mapper) : base(userService, exceptionService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _locationService = locationService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        [CheckUser]
        public IActionResult Index()
        {
            return TryGetActionResult(() =>
            {
                var showDeletedLocations = Session.GetBool("ShowDeletedLocations");
                var showDeletedLocationTypes = Session.GetBool("ShowDeletedLocationTypes");

                var locationsResult = _locationService.GetAll(showDeletedLocations);

                if (locationsResult.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить список мест.";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ShowDeletedLocations = showDeletedLocations;
                ViewBag.ShowDeletedLocationTypes = showDeletedLocationTypes;

                ClearAttachments();

                return View(_mapper.Map<IEnumerable<LocationIndexViewModel>>(locationsResult.Result));
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Home");
            });          
        }

        [HttpGet]
        [CheckUser]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedLocations", show);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [CheckUser]
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
        [CheckUser]
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
        [CheckUser]
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

                var viewModel = _mapper.Map<LocationDetailsViewModel>(result.Result);
                viewModel.Deliveries.Mode = DeliveryTableMode.LocationDetails;

                return View(viewModel);
            }, OnFault);
        }

        [HttpGet]
        [CheckUser]
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
        [CheckUser]
        [LoadAttachments]
        [ValidateModel]        
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

                if (MemoryCache.TryGetValue(CacheKeys.CreateLocationCacheKey, out DeliveryCreateUpdateViewModel cachedDelivery))
                {
                    cachedDelivery.LocationName = model.Name;
                    cachedDelivery.LocationID = result.Result;
                    MemoryCache.Set(CacheKeys.CreateLocationCacheKey, cachedDelivery);
                    Session.SetBool(CacheKeys.CreateLocationCacheValidKey, true);
                }
                else
                {
                    return BadRequest();
                }

                return Redirect(model.ReturnPath);
            }, OnFault);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckUser]
        [LoadAttachments]
        [ValidateModel]
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
        [CheckUser]
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
        [CheckUser]
        [LoadAttachments]
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
        [CheckUser]
        public IActionResult Cancel(LocationCreateUpdateViewModel model)
        {
            if (string.IsNullOrEmpty(model.ReturnPath))
            {
                return RedirectToAction("Index", "Location");
            }

            if (MemoryCache.TryGetValue(CacheKeys.CreateLocationCacheKey, out DeliveryCreateUpdateViewModel _))
            {
                Session.SetBool(CacheKeys.CreateLocationCacheValidKey, true);
            }

            return Redirect(model.ReturnPath);
        }

        #endregion

        [HttpPost]
        [AllowAnonymous]
        public JsonResult GetLocations(string term, int currentLocationId = 0)
        {
            var locations = _locationService.GetAll(false, term, currentLocationId);

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
