using System.Threading.Tasks;
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
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels.Attachment;
using SORANO.WEB.ViewModels.DeliveryItem;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    [CheckUser]
    public class DeliveryItemController : EntityBaseController<DeliveryItemViewModel>
    {
        private readonly IDeliveryItemService _deliveryItemService;
        private readonly IMapper _mapper;

        public DeliveryItemController(IUserService userService,
            IExceptionService exceptionService,
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService,
            IDeliveryItemService deliveryItemService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache,
            IMapper mapper) : base(userService, exceptionService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _deliveryItemService = deliveryItemService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpPost]
        public IActionResult Table(int deliveryId)
        {
            return ViewComponent("DeliveryItemsTable", new { deliveryId });
        }

        [HttpGet]
        public async Task<IActionResult> Create(string returnUrl, int deliveryId)
        {
            return await TryGetActionResultAsync(async () =>
            {
                DeliveryItemViewModel model;

                if (TryGetCached(out var cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
                {
                    model = cachedForSelectMainPicture;
                    await CopyMainPicture(model);
                }
                else if (TryGetCached(out var cachedForCreateArticle, CacheKeys.CreateArticleCacheKey, CacheKeys.CreateArticleCacheValidKey))
                {
                    model = cachedForCreateArticle;
                }
                else
                {
                    model = new DeliveryItemViewModel
                    {
                        MainPicture = new MainPictureViewModel(),
                        ReturnPath = returnUrl,
                        Quantity = 1,
                        UnitPrice = "0.00",
                        GrossPrice = "0.00",
                        Discount = "0.00",
                        DiscountedPrice = "0.00",
                        DeliveryID = deliveryId
                    };
                }

                return View(model);
            }, ex =>
            {
                TempData["Error"] = ex;
                return Redirect(returnUrl);
            });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string returnUrl, int deliveryItemId)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _deliveryItemService.GetAsync(deliveryItemId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанную позицию поставки.";
                    return Redirect(returnUrl);
                }

                var model = _mapper.Map<DeliveryItemViewModel>(result.Result);
                model.ReturnPath = returnUrl;

                return View(model);
            }, ex =>
            {
                TempData["Error"] = ex;
                return Redirect(returnUrl);
            });
        }

        [HttpGet]
        public async Task<IActionResult> Update(string returnUrl, int deliveryItemId)
        {
            return await TryGetActionResultAsync(async () =>
            {
                DeliveryItemViewModel model;

                if (TryGetCached(out var cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
                {
                    model = cachedForSelectMainPicture;
                    await CopyMainPicture(model);
                }
                else if (TryGetCached(out var cachedForCreateArticle, CacheKeys.CreateArticleCacheKey, CacheKeys.CreateArticleCacheValidKey))
                {
                    model = cachedForCreateArticle;
                }
                else
                {
                    var result = await _deliveryItemService.GetAsync(deliveryItemId);

                    if (result.Status != ServiceResponseStatus.Success)
                    {
                        TempData["Error"] = "Не удалось получить позицию поставки.";
                        return Redirect(returnUrl);
                    }

                    model = _mapper.Map<DeliveryItemViewModel>(result.Result);

                    model.ReturnPath = returnUrl;
                    model.IsUpdate = true;
                }
               
                return View("Create", model);
            }, ex =>
            {
                TempData["Error"] = ex;
                return Redirect(returnUrl);
            });
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public IActionResult CreateArticle(DeliveryItemViewModel model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return TryGetActionResult(() =>
            {
                MemoryCache.Set(CacheKeys.CreateArticleCacheKey, model);
                return RedirectToAction("Create", "Article", new { returnUrl });
            }, ex =>
            {
                TempData["Error"] = ex;
                return View("Create", model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        [ValidateModel]
        public async Task<IActionResult> Create(DeliveryItemViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            if (string.IsNullOrWhiteSpace(model.ReturnPath))
                return BadRequest();

            return await TryGetActionResultAsync(async () =>
            {
                if (!ModelState.IsValid)
                    return View(model);

                var deliveryItem = _mapper.Map<DeliveryItemDto>(model);

                var result = await _deliveryItemService.CreateAsync(deliveryItem, UserId);
                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось создать позицию поставки.";
                    return Redirect(model.ReturnPath);
                }

                TempData["Success"] = "Позиция поставки была успешно добавлена.";
                return Redirect(model.ReturnPath);
            }, ex =>
            {
                TempData["Error"] = ex;
                return Redirect(model.ReturnPath);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        [ValidateModel]
        public async Task<IActionResult> Update(DeliveryItemViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            if (string.IsNullOrWhiteSpace(model.ReturnPath))
                return BadRequest();

            return await TryGetActionResultAsync(async () =>
            {
                if (!ModelState.IsValid)
                    return View("Create", model);

                var deliveryItem = _mapper.Map<DeliveryItemDto>(model);

                var result = await _deliveryItemService.UpdateAsync(deliveryItem, UserId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось обновить позицию поставки.";
                    return Redirect(model.ReturnPath);
                }

                TempData["Success"] = "Позиция поставки была успешно обновлена.";
                return Redirect(model.ReturnPath);
            }, ex =>
            {
                TempData["Error"] = ex;
                return Redirect(model.ReturnPath);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeliveryItemViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _deliveryItemService.DeleteAsync(model.ID, UserId);

                if (result.Status == ServiceResponseStatus.Success)
                {
                    TempData["Success"] = "Позиция была успешно удалена из поставки.";
                }
                else
                {
                    TempData["Error"] = "Не удалось удалить позицию из поставки.";
                }

                return Redirect(model.ReturnPath);
            }, ex =>
            {
                TempData["Error"] = ex;
                return Redirect(model.ReturnPath);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(DeliveryItemViewModel model)
        {
            if (string.IsNullOrEmpty(model.ReturnPath))
            {
                return BadRequest();
            }

            return Redirect(model.ReturnPath);
        }

        #endregion
    }
}