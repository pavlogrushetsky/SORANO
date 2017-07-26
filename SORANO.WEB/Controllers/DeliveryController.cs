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
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager,editor,user")]
    public class DeliveryController : EntityBaseController<DeliveryModel>
    {
        private readonly IDeliveryService _deliveryService;
        private readonly ISupplierService _supplierService;
        private readonly IArticleService _articleService;
        private readonly ILocationService _locationService;

        public DeliveryController(IUserService userService,
            IHostingEnvironment hostingEnvironment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache, 
            IDeliveryService deliveryService,
            ISupplierService supplierService,
            IArticleService articleService,
            ILocationService locationService) : base(userService, hostingEnvironment, attachmentTypeService, attachmentService, memoryCache)
        {
            _deliveryService = deliveryService;
            _supplierService = supplierService;
            _articleService = articleService;
            _locationService = locationService;
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

            if (TryGetCached(out DeliveryModel cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
            {
                model = cachedForSelectMainPicture;
                await CopyMainPicture(model);
            }
            else if (TryGetCached(out DeliveryModel cachedForCreateSupplier, CacheKeys.CreateSupplierCacheKey, CacheKeys.CreateSupplierCacheValidKey))
            {
                model = cachedForCreateSupplier;
            }
            else if (TryGetCached(out DeliveryModel cachedForCreateLocation, CacheKeys.CreateLocationCacheKey, CacheKeys.CreateLocationCacheValidKey))
            {
                model = cachedForCreateLocation;
            }
            else if (TryGetCached(out DeliveryModel cachedForCreateArticle, CacheKeys.CreateArticleCacheKey, CacheKeys.CreateArticleCacheValidKey))
            {
                model = cachedForCreateArticle;
            }
            else
            {
                model = new DeliveryModel
                {
                    MainPicture = new AttachmentModel()
                };
            }

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewBag.Suppliers = await GetSuppliers();
            ViewBag.Locations = await GetLocations();
            ViewBag.Articles = await GetArticles();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            DeliveryModel model;

            if (TryGetCached(out DeliveryModel cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
            {
                model = cachedForSelectMainPicture;
                await CopyMainPicture(model);
            }
            else if (TryGetCached(out DeliveryModel cachedForCreateSupplier, CacheKeys.CreateSupplierCacheKey, CacheKeys.CreateSupplierCacheValidKey))
            {
                model = cachedForCreateSupplier;
            }
            else if (TryGetCached(out DeliveryModel cachedForCreateLocation, CacheKeys.CreateLocationCacheKey, CacheKeys.CreateLocationCacheValidKey))
            {
                model = cachedForCreateLocation;
            }
            else
            {
                var delivery = await _deliveryService.GetIncludeAllAsync(id);

                model = delivery.ToModel();
            }


            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewBag.Suppliers = await GetSuppliers();
            ViewBag.Locations = await GetLocations();
            ViewBag.Articles = await GetArticles();

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

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLocation(DeliveryModel model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            await LoadMainPicture(model, mainPictureFile);
            await LoadAttachments(model, attachments);

            _memoryCache.Set(CacheKeys.CreateLocationCacheKey, model);

            return RedirectToAction("Create", "Location", new { returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSupplier(DeliveryModel model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            await LoadMainPicture(model, mainPictureFile);
            await LoadAttachments(model, attachments);

            _memoryCache.Set(CacheKeys.CreateSupplierCacheKey, model);

            return RedirectToAction("Create", "Supplier", new { returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArticle(DeliveryModel model, int num, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            await LoadMainPicture(model, mainPictureFile);
            await LoadAttachments(model, attachments);

            model.CurrentItemNumber = num;

            _memoryCache.Set(CacheKeys.CreateArticleCacheKey, model);

            return RedirectToAction("Create", "Article", new { returnUrl });
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddDeliveryItem(DeliveryModel delivery, bool isEdit, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            ModelState.Clear();

            delivery.DeliveryItems.Add(new DeliveryItemModel
            {
                Quantity = 1
            });

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewBag.Articles = await GetArticles();
            ViewBag.Suppliers = await GetSuppliers();
            ViewBag.Locations = await GetLocations();

            ViewData["IsEdit"] = isEdit;

            await LoadMainPicture(delivery, mainPictureFile);
            await LoadAttachments(delivery, attachments);

            return View("Create", delivery);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteDeliveryItem(DeliveryModel delivery, bool isEdit, int num, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            ModelState.Clear();

            delivery.DeliveryItems.RemoveAt(num);

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewBag.Articles = await GetArticles();
            ViewBag.Suppliers = await GetSuppliers();
            ViewBag.Locations = await GetLocations();

            ViewData["IsEdit"] = isEdit;

            await LoadMainPicture(delivery, mainPictureFile);
            await LoadAttachments(delivery, attachments);

            return View("Create", delivery);
        }

        [HttpPost]
        public virtual async Task<string> GetArticleName(int id)
        {
            if (id <= 0)
            {
                return string.Empty;
            }

            var article = await _articleService.GetAsync(id);

            return article.Name;
        }

        #endregion

        private async Task<List<SelectListItem>> GetSuppliers()
        {
            var suppliers = await _supplierService.GetAllAsync(false);

            var supplierItems = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "0",
                    Text = "-- Поставщик --"
                }
            };

            supplierItems.AddRange(suppliers.Select(s => new SelectListItem
            {
                Value = s.ID.ToString(),
                Text = s.Name
            }));

            _memoryCache.Set(CacheKeys.SuppliersCacheKey, supplierItems);

            return supplierItems;
        }

        private async Task<List<SelectListItem>> GetLocations()
        {
            var locations = await _locationService.GetAllAsync(false);

            var locationItems = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "0",
                    Text = "-- Место поставки --"
                }
            };

            locationItems.AddRange(locations.Select(l => new SelectListItem
            {
                Value = l.ID.ToString(),
                Text = l.Name
            }));

            _memoryCache.Set(CacheKeys.LocationsCacheKey, locationItems);

            return locationItems;
        }

        private async Task<List<SelectListItem>> GetArticles()
        {
            var articles = await _articleService.GetAllAsync(false);

            var articleItems = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "0",
                    Text = "-- Артикул --"
                }
            };

            articleItems.AddRange(articles.Select(l => new SelectListItem
            {
                Value = l.ID.ToString(),
                Text = l.Code
            }));

            _memoryCache.Set(CacheKeys.ArticlesCacheKey, articleItems);

            return articleItems;
        }
    }
}