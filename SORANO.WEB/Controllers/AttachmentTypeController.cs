using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using SORANO.BLL.Services;
using SORANO.WEB.ViewModels;
using SORANO.WEB.ViewModels.AttachmentType;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class AttachmentTypeController : EntityBaseController<AttachmentTypeCreateUpdateViewModel>
    {
        private readonly IMapper _mapper;

        public AttachmentTypeController(IAttachmentTypeService attachmentTypeService, 
            IUserService userService,
            IHostingEnvironment environment,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache, 
            IMapper mapper) : base(userService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await AttachmentTypeService.GetAllAsync(true);

                if (result.Status != ServiceResponseStatusType.Fail)
                {
                    return View(_mapper.Map<IEnumerable<AttachmentTypeIndexViewModel>>(result.Result));
                }

                TempData["Error"] = result.Message;
                return RedirectToAction("Index", "Home");
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Home");
            });          
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AttachmentTypeCreateUpdateViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await AttachmentTypeService.GetAsync(id);

                if (result.Status == ServiceResponseStatusType.Fail)
                {
                    TempData["Error"] = result.Message;
                    return RedirectToAction("Index");
                }

                var model = _mapper.Map<AttachmentTypeCreateUpdateViewModel>(result.Result);
                model.IsUpdate = true;

                return View("Create", model);
            }, OnFault);           
        }       

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await AttachmentTypeService.GetAsync(id);

                if (result.Status != ServiceResponseStatusType.Fail)
                {
                    return View(_mapper.Map<AttachmentTypeDeleteViewModel>(result.Result));
                }

                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }, OnFault);            
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttachmentTypeCreateUpdateViewModel model)
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
            }, OnFault);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AttachmentTypeCreateUpdateViewModel model)
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
            }, OnFault);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(AttachmentTypeDeleteViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var currentUser = await GetCurrentUser();

                var result = await AttachmentTypeService.DeleteAsync(model.ID, currentUser.ID);

                if (result.Status == ServiceResponseStatusType.Success)
                {
                    TempData["Success"] = $"Тип вложений \"{model.Name}\" был успешно помечен как удалённый";
                }
                else
                {
                    TempData["Error"] = result.Message;
                }

                return RedirectToAction("Index");
            }, OnFault);
        }

        #endregion

        private IActionResult OnFault(string ex)
        {
            TempData["Error"] = ex;
            return RedirectToAction("Index");
        }
    }
}
