using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Models.AttachmentType;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class AttachmentTypeController : EntityBaseController<AttachmentTypeModel>
    {
        public AttachmentTypeController(IAttachmentTypeService attachmentTypeService, 
            IUserService userService,
            IHostingEnvironment environment,
            IMemoryCache memoryCache) : base(userService, environment, attachmentTypeService, memoryCache)
        {
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var attachmentTypes = await _attachmentTypeService.GetAllAsync();

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
            var attachmentType = await _attachmentTypeService.GetAsync(id);

            ViewData["IsEdit"] = true;

            return View("Create", attachmentType.ToModel());
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var attachmentType = await _attachmentTypeService.GetAsync(id);

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
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var attachmentType = model.ToEntity();

                var currentUser = await GetCurrentUser();

                attachmentType = await _attachmentTypeService.CreateAsync(attachmentType, currentUser.ID);

                if (attachmentType != null)
                {
                    await CacheAttachmentTypes();
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
                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    return View("Create", model);
                }

                var attachmentType = model.ToEntity();

                var currentUser = await GetCurrentUser();

                attachmentType = await _attachmentTypeService.UpdateAsync(attachmentType, currentUser.ID);

                if (attachmentType != null)
                {
                    await CacheAttachmentTypes();
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

                await _attachmentTypeService.DeleteAsync(model.ID, currentUser.ID);

                return RedirectToAction("Index", "AttachmentType");
            }, ex => RedirectToAction("Index", "AttachmentType"));
        }

        #endregion
    }
}
