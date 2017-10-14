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
using SORANO.WEB.ViewModels.Supplier;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    [CheckUserFilter]
    public class SupplierController : EntityBaseController<SupplierCreateUpdateViewModel>
    {
        private readonly ISupplierService _supplierService;
        private readonly IMapper _mapper;

        public SupplierController(ISupplierService supplierService, 
            IUserService userService,
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache,
            IMapper mapper) : base(userService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _supplierService = supplierService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await TryGetActionResultAsync(async () =>
            {
                bool showDeleted = Session.GetBool("ShowDeletedSuppliers");

                var suppliersResult = await _supplierService.GetAllAsync(showDeleted);

                if (suppliersResult.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить список поставщиков.";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ShowDeleted = showDeleted;

                await ClearAttachments();

                return View(_mapper.Map<IEnumerable<SupplierIndexViewModel>>(suppliersResult.Result));
            }, ex => 
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Home");
            });          
        }

        [HttpGet]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedSuppliers", show);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create(string returnUrl)
        {
            return await TryGetActionResultAsync(async () =>
            {
                SupplierCreateUpdateViewModel model;

                if (TryGetCached(out var cachedModel, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
                {
                    model = cachedModel;
                    await CopyMainPicture(model);
                }
                else
                {
                    model = new SupplierCreateUpdateViewModel
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
                SupplierCreateUpdateViewModel model;

                if (TryGetCached(out var cachedModel, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedModel.ID == id)
                {
                    model = cachedModel;
                    await CopyMainPicture(model);
                }
                else
                {
                    var result = await _supplierService.GetAsync(id);

                    if (result.Status != ServiceResponseStatus.Success)
                    {
                        TempData["Error"] = "Не удалось найти указанного поставщика.";
                        return RedirectToAction("Index");
                    }

                    model = _mapper.Map<SupplierCreateUpdateViewModel>(result.Result);
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
                var result = await _supplierService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанного поставщика.";
                    return RedirectToAction("Index");
                }

                return View(_mapper.Map<SupplierDetailsViewModel>(result.Result));
            }, OnFault);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _supplierService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанного поставщика.";
                    return RedirectToAction("Index");
                }

                return View(_mapper.Map<SupplierDeleteViewModel>(result.Result));
            }, OnFault);
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachmentsFilter]
        [ValidateModelFilter]
        public async Task<IActionResult> Create(SupplierCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var supplier = _mapper.Map<SupplierDto>(model);

                var result = await _supplierService.CreateAsync(supplier, UserId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось создать поставщика.";
                    return RedirectToAction("Index");
                }

                TempData["Success"] = $"Поставщик \"{model.Name}\" был успешно создан.";

                if (string.IsNullOrEmpty(model.ReturnPath))
                {
                    return RedirectToAction("Index");
                }

                // TODO
                //if (MemoryCache.TryGetValue(CacheKeys.CreateSupplierCacheKey, out DeliveryModel cachedDelivery))
                //{
                //    cachedDelivery.Supplier = supplier.ToModel();
                //    cachedDelivery.SupplierID = supplier.ID.ToString();
                //    MemoryCache.Set(CacheKeys.CreateSupplierCacheKey, cachedDelivery);
                //    Session.SetBool(CacheKeys.CreateSupplierCacheValidKey, true);
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
        public async Task<IActionResult> Update(SupplierCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var supplier = _mapper.Map<SupplierDto>(model);

                var result = await _supplierService.UpdateAsync(supplier, UserId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось обновить поставщика.";
                    return RedirectToAction("Index");
                }

                TempData["Success"] = $"Поставщик \"{model.Name}\" был успешно обновлён.";
                return RedirectToAction("Index");
            }, OnFault);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(SupplierDeleteViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _supplierService.DeleteAsync(model.ID, UserId);

                if (result.Status == ServiceResponseStatus.Success)
                {
                    TempData["Success"] = $"Поставщик \"{model.Name}\" был успешно помечен как удалённый.";
                }
                else
                {
                    TempData["Error"] = "Не удалось удалить поставщика.";
                }

                return RedirectToAction("Index");
            }, OnFault);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(SupplierCreateUpdateViewModel model)
        {
            if (string.IsNullOrEmpty(model.ReturnPath))
            {
                return RedirectToAction("Index", "Supplier");
            }

            if (MemoryCache.TryGetValue(CacheKeys.CreateSupplierCacheKey, out DeliveryModel _))
            {
                Session.SetBool(CacheKeys.CreateSupplierCacheValidKey, true);
            }

            return Redirect(model.ReturnPath);
        }

        #endregion

        private IActionResult OnFault(string ex)
        {
            TempData["Error"] = ex;
            return RedirectToAction("Index");
        }
    }
}
