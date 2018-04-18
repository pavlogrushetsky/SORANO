using System;
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
using SORANO.WEB.ViewModels.Delivery;
using System.Threading.Tasks;
using SORANO.WEB.ViewModels.DeliveryItem;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager,editor,user")]
    [CheckUser]
    public class DeliveryController : EntityBaseController<DeliveryCreateUpdateViewModel>
    {
        private readonly IDeliveryService _deliveryService;
        private readonly ILocationService _locationService;
        private readonly ISupplierService _supplierService;
        private readonly IMapper _mapper;

        public DeliveryController(IUserService userService,
            IExceptionService exceptionService,
            IHostingEnvironment hostingEnvironment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache, 
            IDeliveryService deliveryService,
            ISupplierService supplierService,
            IArticleService articleService,
            ILocationService locationService,
            IMapper mapper) : base(userService, exceptionService, hostingEnvironment, attachmentTypeService, attachmentService, memoryCache)
        {
            _deliveryService = deliveryService;
            _locationService = locationService;
            _supplierService = supplierService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await TryGetActionResultAsync(async () => 
            {
                var showDeleted = Session.GetBool("ShowDeletedDeliveries");

                var deliveriesResult = await _deliveryService.GetAllAsync(showDeleted, LocationId);

                if (deliveriesResult.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить список поставок.";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ShowDeleted = showDeleted;

                await ClearAttachments();

                var viewModel = _mapper.Map<DeliveryIndexViewModel>(deliveriesResult.Result);
                viewModel.Mode = DeliveryTableMode.DeliveryIndex;
                viewModel.ShowLocation = !LocationId.HasValue;

                return View(viewModel);
            }, ex => 
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Home");
            });
        }

        [HttpGet]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedDeliveries", show);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return await TryGetActionResultAsync(async () => 
            {
                DeliveryCreateUpdateViewModel model;

                if (TryGetCached(out var cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
                {
                    model = cachedForSelectMainPicture;
                    await CopyMainPicture(model);
                }
                else if (TryGetCached(out var cachedForCreateSupplier, CacheKeys.CreateSupplierCacheKey, CacheKeys.CreateSupplierCacheValidKey))
                {
                    model = cachedForCreateSupplier;
                }
                else if (TryGetCached(out var cachedForCreateLocation, CacheKeys.CreateLocationCacheKey, CacheKeys.CreateLocationCacheValidKey))
                {
                    model = cachedForCreateLocation;
                }
                else
                {
                    int locationId;
                    if (LocationId.HasValue)
                        locationId = LocationId.Value;
                    else
                    {
                        var defaultLocation = await _locationService.GetDefaultLocationAsync();
                        if (defaultLocation.Status != ServiceResponseStatus.Success)
                        {
                            TempData["Error"] = "Не удалось назначить место по умолчанию.";
                            return RedirectToAction("Index");
                        }

                        locationId = defaultLocation.Result.ID;
                    }

                    var defaultSupplier = await _supplierService.GetDefaultSupplierAsync();
                    if (defaultSupplier.Status != ServiceResponseStatus.Success)
                    {
                        TempData["Error"] = "Не удалось назначить поставщика по умолчанию.";
                        return RedirectToAction("Index");
                    }

                    var dto = new DeliveryDto
                    {
                        LocationID = LocationId ?? locationId,
                        SupplierID = defaultSupplier.Result.ID,
                        BillNumber = "<Номер накладной по умолчанию>",
                        DeliveryDate = DateTime.Now
                    };

                    var creationResult = await _deliveryService.CreateAsync(dto, UserId);

                    if (creationResult.Status != ServiceResponseStatus.Success)
                    {
                        TempData["Error"] = "Не удалось создать поставку.";
                        return RedirectToAction("Index");
                    }

                    var delivery = await _deliveryService.GetAsync(creationResult.Result);
                    if (delivery.Status != ServiceResponseStatus.Success)
                    {
                        TempData["Error"] = "Не удалось найти указанную поставку.";
                        return RedirectToAction("Index");
                    }

                    model = _mapper.Map<DeliveryCreateUpdateViewModel>(delivery.Result);
                    model.AllowCreation = AllowCreation;
                    model.AllowChangeLocation = true;
                    model.MainPicture = new MainPictureViewModel();                  

                    TempData["Success"] = "Поставка была успешно оформлена и инициализирована значениями по умолчанию. Для отмены нажмите \"Отменить поставку\". Для подтверждения оформления нажмите \"Подтвердить\".";
                }

                return View(model);
            }, OnFault);           
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            return await TryGetActionResultAsync(async () => 
            {
                DeliveryCreateUpdateViewModel model;

                if (TryGetCached(out var cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
                {
                    model = cachedForSelectMainPicture;
                    await CopyMainPicture(model);
                }
                else if (TryGetCached(out var cachedForCreateSupplier, CacheKeys.CreateSupplierCacheKey, CacheKeys.CreateSupplierCacheValidKey))
                {
                    model = cachedForCreateSupplier;
                }
                else if (TryGetCached(out var cachedForCreateLocation, CacheKeys.CreateLocationCacheKey, CacheKeys.CreateLocationCacheValidKey))
                {
                    model = cachedForCreateLocation;
                }
                else if (TryGetCached(out var cachedForCreateArticle, CacheKeys.CreateArticleCacheKey, CacheKeys.CreateArticleCacheValidKey))
                {
                    model = cachedForCreateArticle;
                }
                else
                {
                    var result = await _deliveryService.GetAsync(id);

                    if (result.Status != ServiceResponseStatus.Success)
                    {
                        TempData["Error"] = "Не удалось найти указанную поставку.";
                        return RedirectToAction("Index");
                    }

                    model = _mapper.Map<DeliveryCreateUpdateViewModel>(result.Result);
                    model.IsUpdate = true;
                    model.AllowChangeLocation = true;
                }

                return View("Create", model);
            }, OnFault);            
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            return await TryGetActionResultAsync(async () => 
            {
                var result = await _deliveryService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанную поставку.";
                    return RedirectToAction("Index");
                }

                var viewModel = _mapper.Map<DeliveryDetailsViewModel>(result.Result);
                viewModel.Table.Mode = DeliveryItemTableMode.DeliveryDetails;

                return View(viewModel);
            }, OnFault);          
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return await TryGetActionResultAsync(async () => 
            {
                var result = await _deliveryService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанную поставку.";
                    return RedirectToAction("Index");
                }

                return View(_mapper.Map<DeliveryDeleteViewModel>(result.Result));
            }, OnFault);          
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public IActionResult CreateLocation(DeliveryCreateUpdateViewModel model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return TryGetActionResult(() =>
            {
                MemoryCache.Set(CacheKeys.CreateLocationCacheKey, model);
                return RedirectToAction("Create", "Location", new { returnUrl });
            }, ex =>
            {
                TempData["Error"] = ex;
                return View("Create", model);
            });           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public IActionResult CreateSupplier(DeliveryCreateUpdateViewModel model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return TryGetActionResult(() =>
            {
                MemoryCache.Set(CacheKeys.CreateSupplierCacheKey, model);
                return RedirectToAction("Create", "Supplier", new { returnUrl });
            }, ex =>
            {
                TempData["Error"] = ex;
                return View("Create", model);
            });            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public IActionResult AddItem(DeliveryCreateUpdateViewModel model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return TryGetActionResult(() => RedirectToAction("Create", "DeliveryItem", new { returnUrl, deliveryId = model.ID }), ex =>
            {
                TempData["Error"] = ex;
                return View("Create", model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public IActionResult UpdateItem(DeliveryCreateUpdateViewModel model, string returnUrl, int deliveryItemId, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return TryGetActionResult(() => RedirectToAction("Update", "DeliveryItem", new { returnUrl, deliveryItemId }), ex =>
            {
                TempData["Error"] = ex;
                return View("Create", model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public IActionResult DeleteItem(DeliveryCreateUpdateViewModel model, string returnUrl, int deliveryItemId, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return TryGetActionResult(() => RedirectToAction("Delete", "DeliveryItem", new {returnUrl, deliveryItemId}), ex =>
            {
                TempData["Error"] = ex;
                return View("Create", model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public async Task<IActionResult> Create(DeliveryCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor(nameof(model.IsSubmitted));

                if (model.IsSubmitted && model.ItemsCount == 0)
                {
                    ModelState.AddModelError("ItemsCount", "Необходимо добавить хотя бы одну позицию");
                }

                if (!ModelState.IsValid)
                {
                    model.IsSubmitted = false;
                    return View(model);
                }

                var delivery = _mapper.Map<DeliveryDto>(model);

                var result = await _deliveryService.UpdateAsync(delivery, UserId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось обновить поставку.";
                    return RedirectToAction("Index");
                }

                TempData["Success"] = "Поставка была успешно обновлена.";
                return RedirectToAction("Index");
            }, OnFault);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public async Task<IActionResult> Update(DeliveryCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor(nameof(model.IsSubmitted));

                if (model.IsSubmitted && model.ItemsCount == 0)
                {
                    ModelState.AddModelError("ItemsCount", "Необходимо добавить хотя бы одну позицию");
                }

                if (!ModelState.IsValid)
                {
                    model.IsSubmitted = false;
                    return View("Create", model);
                }

                var delivery = _mapper.Map<DeliveryDto>(model);

                var result = await _deliveryService.UpdateAsync(delivery, UserId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось обновить поставку.";
                    return RedirectToAction("Index");
                }

                TempData["Success"] = "Поставка была успешно обновлена.";
                return RedirectToAction("Index");
            }, OnFault);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeliveryDeleteViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _deliveryService.DeleteAsync(model.ID, UserId);

                if (result.Status == ServiceResponseStatus.Success)
                {
                    TempData["Success"] = $"Оформление поставки № {model.BillNumber} было успешно отменено.";
                }
                else
                {
                    TempData["Error"] = "Не удалось отменить оформление поставки.";
                }

                return RedirectToAction("Index");
            }, OnFault);
        }

        #endregion

        private IActionResult OnFault(string ex)
        {
            TempData["Error"] = ex;
            return RedirectToAction("Index");
        }      
    }
}