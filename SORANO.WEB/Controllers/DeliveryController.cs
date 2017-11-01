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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager,editor,user")]
    [CheckUserFilter]
    public class DeliveryController : EntityBaseController<DeliveryCreateUpdateViewModel>
    {
        private readonly IDeliveryService _deliveryService;
        private readonly IMapper _mapper;

        public DeliveryController(IUserService userService,
            IHostingEnvironment hostingEnvironment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache, 
            IDeliveryService deliveryService,
            ISupplierService supplierService,
            IArticleService articleService,
            ILocationService locationService,
            IMapper mapper) : base(userService, hostingEnvironment, attachmentTypeService, attachmentService, memoryCache)
        {
            _deliveryService = deliveryService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await TryGetActionResultAsync(async () => 
            {
                var showDeleted = Session.GetBool("ShowDeletedDeliveries");

                var deliveriesResult = await _deliveryService.GetAllAsync(showDeleted);

                if (deliveriesResult.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить список поставок.";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ShowDeleted = showDeleted;

                await ClearAttachments();

                return View(_mapper.Map<IEnumerable<DeliveryIndexViewModel>>(deliveriesResult.Result));
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
                else if (TryGetCached(out var cachedForCreateDeliveryItem, CacheKeys.CreateDeliveryItemCacheKey, CacheKeys.CreateDeliveryItemCacheValidKey))
                {
                    model = cachedForCreateDeliveryItem;
                }
                else
                {
                    model = new DeliveryCreateUpdateViewModel
                    {
                        MainPicture = new MainPictureViewModel()
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
                else if (TryGetCached(out var cachedForCreateItem, CacheKeys.CreateDeliveryItemCacheKey, CacheKeys.CreateDeliveryItemCacheValidKey))
                {
                    model = cachedForCreateItem;
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

                return View(_mapper.Map<DeliveryDetailsViewModel>(result.Result));
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
        [LoadAttachmentsFilter]
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
        [LoadAttachmentsFilter]
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
        [LoadAttachmentsFilter]
        public IActionResult AddItem(DeliveryCreateUpdateViewModel delivery, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return TryGetActionResult(() =>
            {
                MemoryCache.Set(CacheKeys.CreateDeliveryItemCacheKey, delivery);
                return RedirectToAction("Create", "DeliveryItem", new { returnUrl });
            }, ex =>
            {
                TempData["Error"] = ex;
                return View("Create", delivery);
            });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteDeliveryItem(DeliveryCreateUpdateViewModel delivery, int num, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            ModelState.Clear();

            await LoadMainPicture(delivery, mainPictureFile);
            await LoadAttachments(delivery, attachments);

            // TODO
            //delivery.DeliveryItems.RemoveAt(num);          

            return View("Create", delivery);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachmentsFilter]
        public async Task<IActionResult> Create(DeliveryCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor("Status");

                if (model.Status && model.DeliveryItemsCount == 0)
                {
                    ModelState.AddModelError("", "Необходимо добавить хотя бы одну позицию");
                }

                if (!ModelState.IsValid)
                {
                    model.Status = false;
                    return View(model);
                }

                var delivery = _mapper.Map<DeliveryDto>(model);

                var result = await _deliveryService.CreateAsync(delivery, UserId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось создать поставку.";
                    return RedirectToAction("Index");
                }

                TempData["Success"] = "Поставка была успешно создана.";
                return RedirectToAction("Index");
            }, OnFault);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachmentsFilter]
        public async Task<IActionResult> Update(DeliveryCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor("Status");

                if (model.Status && model.DeliveryItemsCount == 0)
                {
                    ModelState.AddModelError("", "Необходимо добавить хотя бы одну позицию");
                }

                if (!ModelState.IsValid)
                {
                    model.Status = false;
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
                    TempData["Success"] = $"Поставка была успешно помечена как удалённая.";
                }
                else
                {
                    TempData["Error"] = "Не удалось удалить поставку.";
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