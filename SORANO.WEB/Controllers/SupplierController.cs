using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Models.Supplier;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeTypes;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Models.Attachment;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class SupplierController : EntityBaseController<SupplierModel>
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService, 
            IUserService userService,
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache) : base(userService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _supplierService = supplierService;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            bool showDeleted = Session.GetBool("ShowDeletedSuppliers");

            var suppliers = await _supplierService.GetAllAsync(showDeleted);

            ViewBag.ShowDeleted = showDeleted;

            await ClearAttachments();

            return View(suppliers.Select(s => s.ToModel()).ToList());
        }

        [HttpGet]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedSuppliers", show);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            SupplierModel model;

            if (TryGetCached(out SupplierModel cachedModel, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
            {
                model = cachedModel;
                await CopyMainPicture(model);
            }
            else
            {
                model = new SupplierModel
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
            SupplierModel model;

            if (TryGetCached(out SupplierModel cachedModel, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedModel.ID == id)
            {
                model = cachedModel;
                await CopyMainPicture(model);
            }
            else
            {
                var supplier = await _supplierService.GetAsync(id);

                model = supplier.ToModel();
            }


            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = true;

            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _supplierService.GetAsync(id);

            return View(supplier.ToModel());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _supplierService.GetIncludeAllAsync(id);

            return View(supplier.ToModel());
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();

            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor("MainPicture");
                attachmentTypes = await GetAttachmentTypes();
                ViewBag.AttachmentTypes = attachmentTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                for (var i = 0; i < model.Attachments.Count; i++)
                {
                    var extensions = model.Attachments[i]
                        .Type.MimeTypes?.Split(',')
                        .Select(MimeTypeMap.GetExtension);

                    if (extensions != null && !extensions.Contains(Path.GetExtension(model.Attachments[i].FullPath)))
                    {
                        ModelState.AddModelError($"Attachments[{i}].Name", "Вложение не соответствует указанному типу");
                    }
                }

                // Check the model
                if (!ModelState.IsValid)
                {
                    ModelState.RemoveDuplicateErrorMessages();
                    return View(model);
                }

                // Convert model to supplier entity
                var supplier = model.ToEntity();

                // Get current user
                var currentUser = await GetCurrentUser();

                // Call correspondent service method to create new supplier
                supplier = await _supplierService.CreateAsync(supplier, currentUser.ID);

                // If succeeded
                if (supplier != null)
                {
                    return RedirectToAction("Index", "Supplier");
                }

                // If failed
                ModelState.AddModelError("", "Не удалось создать нового поставщика.");
                return View(model);
            }, ex =>
            {
                ViewBag.AttachmentTypes = attachmentTypes;
                ModelState.AddModelError("", ex);
                return View(model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(SupplierModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();

            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor("MainPicture");
                attachmentTypes = await GetAttachmentTypes();
                ViewBag.AttachmentTypes = attachmentTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                for (var i = 0; i < model.Attachments.Count; i++)
                {
                    var extensions = model.Attachments[i]
                        .Type.MimeTypes?.Split(',')
                        .Select(MimeTypeMap.GetExtension);

                    if (extensions != null && !extensions.Contains(Path.GetExtension(model.Attachments[i].FullPath)))
                    {
                        ModelState.AddModelError($"Attachments[{i}].Name", "Вложение не соответствует указанному типу");
                    }
                }

                // Check the model
                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    ModelState.RemoveDuplicateErrorMessages();
                    return View("Create", model);
                }

                // Convert model to supplier entity
                var supplier = model.ToEntity();

                // Get current user
                var currentUser = await GetCurrentUser();

                // Call correspondent service method to update supplier
                supplier = await _supplierService.UpdateAsync(supplier, currentUser.ID);

                // If succeeded
                if (supplier != null)
                {
                    return RedirectToAction("Index", "Supplier");
                }

                // If failed
                ModelState.AddModelError("", "Не удалось обновить поставщика.");
                ViewData["IsEdit"] = true;
                return View("Create", model);
            }, ex =>
            {
                ViewBag.AttachmentTypes = attachmentTypes;
                ModelState.AddModelError("", ex);
                ViewData["IsEdit"] = true;
                return View("Create", model);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(SupplierModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var currentUser = await GetCurrentUser();

                await _supplierService.DeleteAsync(model.ID, currentUser.ID);

                return RedirectToAction("Index", "Supplier");
            }, ex => RedirectToAction("Index", "Supplier"));
        }

        #endregion
    }
}
