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
using SORANO.WEB.ViewModels.Supplier;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.WEB.ViewModels.Delivery;

namespace SORANO.WEB.Controllers
{   
    [CheckUser]
    public class SupplierController : EntityBaseController<SupplierCreateUpdateViewModel>
    {
        private readonly ISupplierService _supplierService;
        private readonly IMapper _mapper;

        public SupplierController(ISupplierService supplierService, 
            IUserService userService,
            IExceptionService exceptionService,
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache,
            IMapper mapper) : base(userService, exceptionService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _supplierService = supplierService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        [Authorize(Roles = "developer,administrator,manager,user")]
        public IActionResult Index()
        {
            return TryGetActionResult(() =>
            {
                bool showDeleted = Session.GetBool("ShowDeletedSuppliers");

                var suppliersResult = _supplierService.GetAll(showDeleted);

                if (suppliersResult.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить список поставщиков.";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ShowDeleted = showDeleted;

                ClearAttachments();

                return View(_mapper.Map<IEnumerable<SupplierIndexViewModel>>(suppliersResult.Result));
            }, ex => 
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Home");
            });          
        }

        [HttpGet]
        [Authorize(Roles = "developer,administrator,manager,user")]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedSuppliers", show);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "developer,administrator,manager")]
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
        [Authorize(Roles = "developer,administrator,manager")]
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
        [Authorize(Roles = "developer,administrator,manager,user")]
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

                var viewModel = _mapper.Map<SupplierDetailsViewModel>(result.Result);
                viewModel.Deliveries.Mode = DeliveryTableMode.SupplierDetails;

                return View(viewModel);
            }, OnFault);
        }

        [HttpGet]
        [Authorize(Roles = "developer,administrator,manager")]
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
        [Authorize(Roles = "developer,administrator,manager")]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        [ValidateModel]
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

                if (MemoryCache.TryGetValue(CacheKeys.CreateSupplierCacheKey, out DeliveryCreateUpdateViewModel cachedDelivery))
                {
                    cachedDelivery.SupplierName = model.Name;
                    cachedDelivery.SupplierID = result.Result;
                    MemoryCache.Set(CacheKeys.CreateSupplierCacheKey, cachedDelivery);
                    Session.SetBool(CacheKeys.CreateSupplierCacheValidKey, true);
                }
                else
                {
                    return BadRequest();
                }

                return Redirect(model.ReturnPath);
            }, OnFault);
        }

        [HttpPost]
        [Authorize(Roles = "developer,administrator,manager")]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        [ValidateModel]
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
        [Authorize(Roles = "developer,administrator,manager")]
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
        [Authorize(Roles = "developer,administrator,manager")]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(SupplierCreateUpdateViewModel model)
        {
            if (string.IsNullOrEmpty(model.ReturnPath))
            {
                return RedirectToAction("Index", "Supplier");
            }

            if (MemoryCache.TryGetValue(CacheKeys.CreateSupplierCacheKey, out DeliveryCreateUpdateViewModel _))
            {
                Session.SetBool(CacheKeys.CreateSupplierCacheValidKey, true);
            }

            return Redirect(model.ReturnPath);
        }

        #endregion

        [HttpPost]
        public JsonResult GetSuppliers(string term)
        {
            var suppliers = _supplierService.GetAll(false, term);

            return Json(new
            {
                results = suppliers.Result?
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
            return RedirectToAction("Index");
        }
    }
}
