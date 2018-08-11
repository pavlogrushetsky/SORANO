using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SORANO.BLL.Dtos;
using SORANO.BLL.Services;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels.Attachment;
using SORANO.WEB.ViewModels.Client;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    [CheckUser]
    public class ClientController : EntityBaseController<ClientCreateUpdateViewModel>
    {
        private readonly IClientService _clientService;
        private readonly IMapper _mapper;

        public ClientController(IClientService clientService, 
            IUserService userService,
            IExceptionService exceptionService,
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache, 
            IMapper mapper) : base(userService, exceptionService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _clientService = clientService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        public IActionResult Index()
        {
            return TryGetActionResult(() =>
            {
                var showDeleted = Session.GetBool("ShowDeletedClients");

                var clientsResult = _clientService.GetAll(showDeleted);

                if (clientsResult.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить список клиентов.";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ShowDeleted = showDeleted;

                ClearAttachments();

                return View(_mapper.Map<IEnumerable<ClientIndexViewModel>>(clientsResult.Result));
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Home");
            });
        }

        [HttpGet]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedClients", show);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return await TryGetActionResultAsync(async () =>
            {
                ClientCreateUpdateViewModel model;

                if (TryGetCached(out var cachedModel, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
                {
                    model = cachedModel;
                    await CopyMainPicture(model);
                }
                else
                {
                    model = new ClientCreateUpdateViewModel
                    {
                        MainPicture = new MainPictureViewModel()
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
                ClientCreateUpdateViewModel model;

                if (TryGetCached(out var cachedModel, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedModel.ID == id)
                {
                    model = cachedModel;
                    await CopyMainPicture(model);
                }
                else
                {
                    var client = await _clientService.GetAsync(id);

                    if (client.Status != ServiceResponseStatus.Success)
                    {
                        TempData["Error"] = "Не удалось найти указанного клиента.";
                        return RedirectToAction("Index");
                    }

                    model = _mapper.Map<ClientCreateUpdateViewModel>(client.Result);
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
                var result = await _clientService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанного клиента.";
                    return RedirectToAction("Index");
                }

                return View(_mapper.Map<ClientDetailsViewModel>(result.Result));
            }, OnFault);            
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _clientService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанного клиента.";
                    return RedirectToAction("Index");
                }

                return View(_mapper.Map<ClientDeleteViewModel>(result.Result));
            }, OnFault);
        }       

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        [ValidateModel]
        public async Task<IActionResult> Create(ClientCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var client = _mapper.Map<ClientDto>(model);

                var result = await _clientService.CreateAsync(client, UserId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось создать клиента.";
                    return RedirectToAction("Index");
                }

                TempData["Success"] = $"Клиент \"{model.Name}\" был успешно создан.";
                return RedirectToAction("Index");
            }, OnFault);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        [ValidateModel]
        public async Task<IActionResult> Update(ClientCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var client = _mapper.Map<ClientDto>(model);

                var result = await _clientService.UpdateAsync(client, UserId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось обновить клиента.";
                    return RedirectToAction("Index");
                }

                TempData["Success"] = $"Клиент \"{model.Name}\" был успешно обновлён.";
                return RedirectToAction("Index");
            }, OnFault);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ClientDeleteViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _clientService.DeleteAsync(model.ID, UserId);

                if (result.Status == ServiceResponseStatus.Success)
                {
                    TempData["Success"] = $"Клиент \"{model.Name}\" был успешно помечен как удалённый.";
                }
                else
                {
                    TempData["Error"] = "Не удалось удалить клиента.";
                }

                return RedirectToAction("Index");
            }, OnFault);
        }

        #endregion

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> GetClients(string term)
        {
            var locations = await _clientService.GetAllAsync(false, term);

            return Json(new
            {
                results = locations.Result?
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
