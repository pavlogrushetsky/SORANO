using System.Linq;
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
using SORANO.WEB.ViewModels.Attachment;
using SORANO.WEB.ViewModels.LocationType;
using System.Threading.Tasks;
using SORANO.WEB.ViewModels.Location;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    [CheckUserFilter]
    public class LocationTypeController : EntityBaseController<LocationTypeCreateUpdateViewModel>
    {
        private readonly ILocationTypeService _locationTypeService;
        private readonly IMapper _mapper;

        public LocationTypeController(ILocationTypeService locationTypeService, 
            IUserService userService,
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache, 
            IMapper mapper) : base(userService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _locationTypeService = locationTypeService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedLocationTypes", show);

            return RedirectToAction("Index", "Location");
        }

        [HttpGet]
        public async Task<IActionResult> Create(string returnUrl)
        {
            return await TryGetActionResultAsync(async () =>
            {
                await ClearAttachments();

                LocationTypeCreateUpdateViewModel model;

                if (TryGetCached(out var cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
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

                return View(model);
            }, OnFault);            
        }        

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                await ClearAttachments();

                LocationTypeCreateUpdateViewModel model;

                if (TryGetCached(out var cachedModel, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedModel.ID == id)
                {
                    model = cachedModel;
                    await CopyMainPicture(model);
                }
                else
                {
                    var result = await _locationTypeService.GetAsync(id);

                    if (result.Status != ServiceResponseStatus.Success)
                    {
                        TempData["Error"] = "Не удалось найти указанный тип мест.";
                        return RedirectToAction("Index", "Location");
                    }

                    model = _mapper.Map<LocationTypeCreateUpdateViewModel>(result.Result);
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
                await ClearAttachments();

                var result = await _locationTypeService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанный тип мест.";
                    return RedirectToAction("Index", "Location");
                }

                return View(_mapper.Map<LocationTypeDetailsViewModel>(result.Result));
            }, OnFault);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            await ClearAttachments();

            return await TryGetActionResultAsync(async () =>
            {
                var result = await _locationTypeService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанный тип мест.";
                    return RedirectToAction("Index", "Location");
                }

                return View(_mapper.Map<LocationTypeDeleteViewModel>(result.Result));
            }, OnFault);
        }        

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachmentsFilter]
        [ValidateModelFilter]
        public async Task<IActionResult> Create(LocationTypeCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var locationType = _mapper.Map<LocationTypeDto>(model);

                var result = await _locationTypeService.CreateAsync(locationType, UserId);

                switch (result.Status)
                {
                    case ServiceResponseStatus.NotFound:
                    case ServiceResponseStatus.InvalidOperation:
                        TempData["Error"] = "Не удалось создать тип мест.";
                        return RedirectToAction("Index", "Location");
                    case ServiceResponseStatus.AlreadyExists:
                        ModelState.AddModelError("Name", "Тип мест с таким названием уже существует.");
                        return View(model);
                }

                TempData["Success"] = $"Тип мест \"{model.Name}\" был успешно создан.";

                if (string.IsNullOrEmpty(model.ReturnPath))
                {
                    return RedirectToAction("Index", "Location");
                }

                if (MemoryCache.TryGetValue(CacheKeys.CreateLocationTypeCacheKey, out LocationCreateUpdateViewModel cachedLocation))
                {
                    cachedLocation.TypeName = model.Name;
                    cachedLocation.TypeID = result.Result;
                    MemoryCache.Set(CacheKeys.CreateLocationTypeCacheKey, cachedLocation);
                    Session.SetBool(CacheKeys.CreateLocationTypeCacheValidKey, true);
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
        [LoadAttachmentsFilter]
        [ValidateModelFilter]
        public async Task<IActionResult> Update(LocationTypeCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var locationType = _mapper.Map<LocationTypeDto>(model);

                var result = await _locationTypeService.UpdateAsync(locationType, UserId);

                switch (result.Status)
                {
                    case ServiceResponseStatus.NotFound:
                    case ServiceResponseStatus.InvalidOperation:
                        TempData["Error"] = "Не удалось обновить тип мест.";
                        return RedirectToAction("Index", "Location");
                    case ServiceResponseStatus.AlreadyExists:
                        ModelState.AddModelError("Name", "Тип мест с таким названием уже существует.");
                        return View("Create", model);
                }

                TempData["Success"] = $"Тип мест \"{model.Name}\" был успешно обновлён";
                return RedirectToAction("Index", "Location");
            }, OnFault);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(LocationTypeDeleteViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _locationTypeService.DeleteAsync(model.ID, UserId);

                if (result.Status == ServiceResponseStatus.Success)
                {
                    TempData["Success"] = $"Тип мест \"{model.Name}\" был успешно помечен как удалённый.";
                }
                else
                {
                    TempData["Error"] = "Не удалось удалить тип мест.";
                }

                return RedirectToAction("Index", "Location");
            }, OnFault);
        }       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(LocationTypeCreateUpdateViewModel model)
        {
            if (string.IsNullOrEmpty(model.ReturnPath))
            {
                return RedirectToAction("Index", "Location");
            }

            if (MemoryCache.TryGetValue(CacheKeys.CreateLocationTypeCacheKey, out LocationCreateUpdateViewModel _))
            {
                Session.SetBool(CacheKeys.CreateLocationTypeCacheValidKey, true);
            }

            return Redirect(model.ReturnPath);
        }

        #endregion

        [HttpPost]
        public async Task<JsonResult> GetLocationTypes(string term)
        {
            var locationTypes = await _locationTypeService.GetAllAsync(false, term);

            return Json(new
            {
                results = locationTypes.Result?
                    .Select(t => new
                    {
                        id = t.ID,
                        text = t.Name,
                        desc = t.Description ?? string.Empty
                    })
                    .OrderBy(t => t.text)
            });
        }

        private IActionResult OnFault(string ex)
        {
            TempData["Error"] = ex;
            return RedirectToAction("Index", "Location");
        }
    }
}
