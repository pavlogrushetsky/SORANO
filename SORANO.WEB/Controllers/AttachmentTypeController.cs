using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services.Abstract;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using SORANO.BLL.Dtos;
using SORANO.BLL.Services;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels.AttachmentType;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    [CheckUserFilter]
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
                var result = await AttachmentTypeService.GetAllAsync(true, UserId);

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
                var result = await AttachmentTypeService.GetAsync(id, UserId);

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
                var result = await AttachmentTypeService.GetAsync(id, UserId);

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
                // TODO Move to service
                //var exists = await AttachmentTypeService.Exists(model.Name);

                //if (exists)
                //{
                //    ModelState.AddModelError("Name", "Тип вложений с таким названием уже существует");
                //}

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var attachmentType = _mapper.Map<AttachmentTypeDto>(model);

                var result = await AttachmentTypeService.CreateAsync(attachmentType, UserId);

                if (result.Status == ServiceResponseStatusType.Success)
                {
                    TempData["Success"] = $"Тип вложений \"{model.Name}\" был успешно создан";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = result.Message;
                return View(model);
            }, OnFault);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AttachmentTypeCreateUpdateViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                // TODO Move to service
                //var exists = await AttachmentTypeService.Exists(model.Name, model.ID);

                //if (exists)
                //{
                //    ModelState.AddModelError("Name", "Тип вложений с таким названием уже существует");
                //}

                if (!ModelState.IsValid)
                {
                    return View("Create", model);
                }

                var attachmentType = _mapper.Map<AttachmentTypeDto>(model);

                var result = await AttachmentTypeService.UpdateAsync(attachmentType, UserId);

                if (result.Status == ServiceResponseStatusType.Success)
                {
                    TempData["Success"] = $"Тип вложений \"{model.Name}\" был успешно обновлён";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = result.Message;
                return View("Create", model);
            }, OnFault);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(AttachmentTypeDeleteViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await AttachmentTypeService.DeleteAsync(model.ID, UserId);

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
