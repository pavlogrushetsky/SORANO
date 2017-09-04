﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using SORANO.WEB.ViewModels;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class AttachmentTypeController : EntityBaseController<AttachmentTypeModel>
    {
        public AttachmentTypeController(IAttachmentTypeService attachmentTypeService, 
            IUserService userService,
            IHostingEnvironment environment,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache) : base(userService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await CacheAttachmentTypes();

            var attachmentTypes = await AttachmentTypeService.GetAllAsync();          

            return View(attachmentTypes.Select(a => a.ToModel()).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AttachmentTypeModel());
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var attachmentType = await AttachmentTypeService.GetAsync(id);

            ViewData["IsEdit"] = true;

            return View("Create", attachmentType.ToModel());
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var attachmentType = await AttachmentTypeService.GetAsync(id);

            return View(attachmentType.ToModel());
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttachmentTypeModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var exists = await AttachmentTypeService.Exists(model.Name);

                if (exists)
                {
                    ModelState.AddModelError("Name", "Тип вложений с таким названием уже существует");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var attachmentType = model.ToEntity();

                var currentUser = await GetCurrentUser();

                attachmentType = await AttachmentTypeService.CreateAsync(attachmentType, currentUser.ID);

                if (attachmentType != null)
                {                    
                    return RedirectToAction("Index", "AttachmentType");
                }

                ModelState.AddModelError("", "Не удалось создать новый тип вложений.");
                return View(model);
            }, ex =>
            {
                ModelState.AddModelError("", ex);
                return View(model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AttachmentTypeModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var exists = await AttachmentTypeService.Exists(model.Name, model.ID);

                if (exists)
                {
                    ModelState.AddModelError("Name", "Тип вложений с таким названием уже существует");
                }

                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    return View("Create", model);
                }

                var attachmentType = model.ToEntity();

                var currentUser = await GetCurrentUser();

                attachmentType = await AttachmentTypeService.UpdateAsync(attachmentType, currentUser.ID);

                if (attachmentType != null)
                {
                    return RedirectToAction("Index", "AttachmentType");
                }

                ModelState.AddModelError("", "Не удалось обновить тип вложений.");
                ViewData["IsEdit"] = true;
                return View("Create", model);
            }, ex =>
            {
                ModelState.AddModelError("", ex);
                ViewData["IsEdit"] = true;
                return View("Create", model);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(AttachmentTypeModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var currentUser = await GetCurrentUser();

                await AttachmentTypeService.DeleteAsync(model.ID, currentUser.ID);

                return RedirectToAction("Index", "AttachmentType");
            }, ex => RedirectToAction("Index", "AttachmentType"));
        }

        #endregion
    }
}
