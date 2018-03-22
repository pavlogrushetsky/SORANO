using System.Collections.Generic;
using System.Linq;
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
    [CheckUser]
    public class AttachmentTypeController : EntityBaseController<AttachmentTypeCreateUpdateViewModel>
    {
        private readonly IMapper _mapper;

        public AttachmentTypeController(IAttachmentTypeService attachmentTypeService, 
            IUserService userService,
            IExceptionService exceptionService,
            IHostingEnvironment environment,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache, 
            IMapper mapper) : base(userService, exceptionService, environment, attachmentTypeService, attachmentService, memoryCache)
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

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить список типов вложений.";
                    return RedirectToAction("Index", "Home");
                }
               
                return View(_mapper.Map<IEnumerable<AttachmentTypeIndexViewModel>>(result.Result));
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

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанный тип вложений.";
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

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанный тип вложений.";
                    return RedirectToAction("Index");
                }
                
                return View(_mapper.Map<AttachmentTypeDeleteViewModel>(result.Result));
            }, OnFault);            
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModel]
        public async Task<IActionResult> Create(AttachmentTypeCreateUpdateViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var attachmentType = _mapper.Map<AttachmentTypeCreateUpdateViewModel, AttachmentTypeDto>(model);

                var result = await AttachmentTypeService.CreateAsync(attachmentType, UserId);

                switch (result.Status)
                {
                    case ServiceResponseStatus.NotFound:
                    case ServiceResponseStatus.InvalidOperation:
                        TempData["Error"] = "Не удалось создать тип вложений.";
                        return RedirectToAction("Index");
                    case ServiceResponseStatus.AlreadyExists:
                        ModelState.AddModelError("Name", "Тип вложений с таким названием уже существует.");
                        return View(model);
                }

                TempData["Success"] = $"Тип вложений \"{model.Name}\" был успешно создан.";
                return RedirectToAction("Index");
            }, OnFault);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModel]
        public async Task<IActionResult> Update(AttachmentTypeCreateUpdateViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var attachmentType = _mapper.Map<AttachmentTypeDto>(model);

                var result = await AttachmentTypeService.UpdateAsync(attachmentType, UserId);

                switch (result.Status)
                {
                    case ServiceResponseStatus.NotFound:
                    case ServiceResponseStatus.InvalidOperation:
                        TempData["Error"] = "Не удалось обновить тип вложений.";
                        return RedirectToAction("Index");
                    case ServiceResponseStatus.AlreadyExists:
                        ModelState.AddModelError("Name", "Тип вложений с таким названием уже существует.");
                        return View("Create", model);
                }

                TempData["Success"] = $"Тип вложений \"{model.Name}\" был успешно обновлён.";
                return RedirectToAction("Index");
            }, OnFault);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(AttachmentTypeDeleteViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await AttachmentTypeService.DeleteAsync(model.ID, UserId);

                if (result.Status == ServiceResponseStatus.Success)
                {
                    TempData["Success"] = $"Тип вложений \"{model.Name}\" был успешно помечен как удалённый.";
                }
                else
                {
                    TempData["Error"] = "Не удалось удалить тип вложений.";
                }

                return RedirectToAction("Index");
            }, OnFault);
        }

        #endregion

        [HttpPost]
        public async Task<JsonResult> GetAttachmentTypes(string term)
        {
            var attachmentTypes = await AttachmentTypeService.GetAllAsync(term);

            return Json(new
            {
                results = attachmentTypes.Result?
                    .Select(t => new
                    {
                        id = t.ID,
                        text = t.Name,
                        exts = t.Extensions,
                        desc = t.Comment
                    })
                    .OrderBy(t => t.text)
            });
        }

        [HttpPost]
        public virtual async Task<string> GetMimeTypes(int id)
        {
            var result = await AttachmentTypeService.GetAsync(id);

            if (result.Status != ServiceResponseStatus.Success)
                return string.Empty;

            var model = _mapper.Map<AttachmentTypeIndexViewModel>(result.Result);
            return model.MimeTypes;
        }

        private IActionResult OnFault(string ex)
        {
            TempData["Error"] = ex;
            return RedirectToAction("Index");
        }
    }
}
