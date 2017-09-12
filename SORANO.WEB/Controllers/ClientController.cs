using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels;
using SORANO.WEB.ViewModels.Attachment;
using SORANO.WEB.ViewModels.Client;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    [CheckUserFilter]
    public class ClientController : EntityBaseController<ClientCreateUpdateViewModel>
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService, 
            IUserService userService,
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache) : base(userService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _clientService = clientService;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            bool showDeleted = Session.GetBool("ShowDeletedClients");

            var clients = await _clientService.GetAllAsync(showDeleted, UserId);

            ViewBag.ShowDeleted = showDeleted;

            await ClearAttachments();

            return View(clients.Select(s => s.ToModel()).ToList());
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
            ClientCreateUpdateViewModel model;

            if (TryGetCached(out ClientCreateUpdateViewModel cachedModel, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
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

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            ClientCreateUpdateViewModel model;

            if (TryGetCached(out ClientCreateUpdateViewModel cachedModel, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedModel.ID == id)
            {
                model = cachedModel;
                await CopyMainPicture(model);
            }
            else
            {
                var client = await _clientService.GetAsync(id, UserId);

                model = client.ToModel();
            }


            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = true;

            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _clientService.GetAsync(id, UserId);

            return View(client.ToModel());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var client = await _clientService.GetIncludeAllAsync(id);

            return View(client.ToModel());
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();

            return await TryGetActionResultAsync(async () =>
            {
                attachmentTypes = await GetAttachmentTypes();
                ViewBag.AttachmentTypes = attachmentTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var client = model.ToEntity();

                client = await _clientService.CreateAsync(client, UserId);

                if (client != null)
                {
                    return RedirectToAction("Index", "Client");
                }

                ModelState.AddModelError("", "Не удалось создать нового клиента.");
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
        public async Task<IActionResult> Update(ClientCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();

            return await TryGetActionResultAsync(async () =>
            {
                attachmentTypes = await GetAttachmentTypes();
                ViewBag.AttachmentTypes = attachmentTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    return View("Create", model);
                }

                var client = model.ToEntity();

                client = await _clientService.UpdateAsync(client, UserId);

                if (client != null)
                {
                    return RedirectToAction("Index", "Client");
                }

                ModelState.AddModelError("", "Не удалось обновить клиента.");
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
        public async Task<IActionResult> Delete(ClientModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                await _clientService.DeleteAsync(model.ID, UserId);

                return RedirectToAction("Index", "Client");
            }, ex => RedirectToAction("Index", "Client"));
        }

        #endregion
    }
}
