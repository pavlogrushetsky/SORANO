using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels;
using SORANO.WEB.ViewModels.Attachment;
using SORANO.WEB.ViewModels.DeliveryItem;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    [CheckUserFilter]
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
        public IActionResult Create(string returnUrl)
        {
            return TryGetActionResult(() => View(new DeliveryItemViewModel
            {
                MainPicture = new MainPictureViewModel(),
                ReturnPath = returnUrl
            }), ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Delivery");
            });
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(DeliveryItemViewModel model)
        {
            if (string.IsNullOrEmpty(model.ReturnPath))
            {
                return RedirectToAction("Index", "Delivery");
            }

            if (MemoryCache.TryGetValue(CacheKeys.CreateDeliveryItemCacheKey, out DeliveryModel _))
            {
                Session.SetBool(CacheKeys.CreateDeliveryItemCacheValidKey, true);
            }

            return Redirect(model.ReturnPath);
        }

        #endregion
    }
}