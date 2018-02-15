using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels.Attachment;
using SORANO.WEB.ViewModels.Delivery;
using SORANO.WEB.ViewModels.DeliveryItem;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    [CheckUser]
    public class DeliveryItemController : EntityBaseController<DeliveryItemViewModel>
    {
        public DeliveryItemController(IUserService userService,
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache) : base(userService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Create(string returnUrl)
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
                        DiscountedPrice = "0.00"
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
        public async Task<IActionResult> Update(int number, string returnUrl)
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
                else if (TryGetCached(out var cachedItem, CacheKeys.DeliveryItemCacheKey, CacheKeys.DeliveryItemCacheValidKey))
                {
                    cachedItem.Number = number;
                    cachedItem.ReturnPath = returnUrl;
                    cachedItem.IsUpdate = true;

                    model = cachedItem;
                }
                else
                {
                    TempData["Error"] = "Не удалось получить позицию поставки.";
                    return Redirect(returnUrl);
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
        public IActionResult Create(DeliveryItemViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            if (string.IsNullOrWhiteSpace(model.ReturnPath))
                return BadRequest();

            return TryGetActionResult(() =>
            {
                TempData["Success"] = "Позиция поставки была успешно добавлена.";

                if (MemoryCache.TryGetValue(CacheKeys.CreateDeliveryItemCacheKey, out DeliveryCreateUpdateViewModel cachedDelivery))
                {
                    cachedDelivery.Items.Add(model);
                    MemoryCache.Set(CacheKeys.CreateDeliveryItemCacheKey, cachedDelivery);
                    Session.SetBool(CacheKeys.CreateDeliveryItemCacheValidKey, true);
                }
                else
                {
                    TempData["Error"] = "Не удалось создать позицию поставки.";
                    return Redirect(model.ReturnPath);
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
        [LoadAttachments]
        [ValidateModel]
        public IActionResult Update(DeliveryItemViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            if (string.IsNullOrWhiteSpace(model.ReturnPath))
                return BadRequest();

            return TryGetActionResult(() =>
            {               
                if (MemoryCache.TryGetValue(CacheKeys.CreateDeliveryItemCacheKey, out DeliveryCreateUpdateViewModel cachedDelivery))
                {
                    var item = model.ID > 0
                        ? cachedDelivery.Items.SingleOrDefault(di => di.ID == model.ID)
                        : cachedDelivery.Items[model.Number];

                    if (item == null)
                    {
                        TempData["Error"] = "Не удалось обновить позицию поставки.";
                        return Redirect(model.ReturnPath);
                    }

                    var index = cachedDelivery.Items.IndexOf(item);
                    cachedDelivery.Items[index] = model;
                    MemoryCache.Set(CacheKeys.CreateDeliveryItemCacheKey, cachedDelivery);
                    Session.SetBool(CacheKeys.CreateDeliveryItemCacheValidKey, true);
                }
                else
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
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(DeliveryItemViewModel model)
        {
            if (string.IsNullOrEmpty(model.ReturnPath))
            {
                return BadRequest();
            }

            if (MemoryCache.TryGetValue(CacheKeys.CreateDeliveryItemCacheKey, out DeliveryCreateUpdateViewModel _))
            {
                Session.SetBool(CacheKeys.CreateDeliveryItemCacheValidKey, true);
            }

            return Redirect(model.ReturnPath);
        }

        #endregion
    }
}