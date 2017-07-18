using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Models;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager,editor,user")]
    public class DeliveryController : EntityBaseController<DeliveryModel>
    {
        private readonly IDeliveryService _deliveryService;

        public DeliveryController(IUserService userService,
            IHostingEnvironment hostingEnvironment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache, 
            IDeliveryService deliveryService) : base(userService, hostingEnvironment, attachmentTypeService, attachmentService, memoryCache)
        {
            _deliveryService = deliveryService;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var showDeleted = Session.GetBool("ShowDeletedDeliveries");

            var deliveries = await _deliveryService.GetAllAsync(showDeleted);

            ViewBag.ShowDeleted = showDeleted;

            await ClearAttachments();

            return View(deliveries.Select(d => d.ToModel()).ToList());
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
            DeliveryModel model;

            if (TryGetCached(out DeliveryModel cachedModel, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
            {
                model = cachedModel;
                await CopyMainPicture(model);
            }
            else
            {
                model = new DeliveryModel
                {
                    MainPicture = new AttachmentModel()
                };
            }

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            DeliveryModel model;

            if (TryGetCached(out DeliveryModel cachedModel, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedModel.ID == id)
            {
                model = cachedModel;
                await CopyMainPicture(model);
            }
            else
            {
                var delivery = await _deliveryService.GetIncludeAllAsync(id);

                model = delivery.ToModel();
            }


            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = true;

            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var delivery = await _deliveryService.GetIncludeAllAsync(id);

            return View(delivery.ToModel());
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var delivery = await _deliveryService.GetIncludeAllAsync(id);

            return View(delivery.ToModel());
        }

        #endregion        
    }
}