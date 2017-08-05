using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeTypes;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Models;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class ClientController : EntityBaseController<ClientModel>
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

            var clients = await _clientService.GetAllAsync(showDeleted);

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
            ClientModel model;

            if (TryGetCached(out ClientModel cachedModel, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
            {
                model = cachedModel;
                await CopyMainPicture(model);
            }
            else
            {
                model = new ClientModel
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
            ClientModel model;

            if (TryGetCached(out ClientModel cachedModel, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedModel.ID == id)
            {
                model = cachedModel;
                await CopyMainPicture(model);
            }
            else
            {
                var client = await _clientService.GetAsync(id);

                model = client.ToModel();
            }


            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = true;

            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _clientService.GetAsync(id);

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
        public async Task<IActionResult> Create(ClientModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();

            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                attachmentTypes = await GetAttachmentTypes();
                ViewBag.AttachmentTypes = attachmentTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                // Check the model
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var client = model.ToEntity();

                // Get current user
                var currentUser = await GetCurrentUser();

                client = await _clientService.CreateAsync(client, currentUser.ID);

                // If succeeded
                if (client != null)
                {
                    return RedirectToAction("Index", "Client");
                }

                // If failed
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
        public async Task<IActionResult> Update(ClientModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();

            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                attachmentTypes = await GetAttachmentTypes();
                ViewBag.AttachmentTypes = attachmentTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                // Check the model
                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    return View("Create", model);
                }

                var client = model.ToEntity();

                // Get current user
                var currentUser = await GetCurrentUser();

                client = await _clientService.UpdateAsync(client, currentUser.ID);

                // If succeeded
                if (client != null)
                {
                    return RedirectToAction("Index", "Client");
                }

                // If failed
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
                var currentUser = await GetCurrentUser();

                await _clientService.DeleteAsync(model.ID, currentUser.ID);

                return RedirectToAction("Index", "Client");
            }, ex => RedirectToAction("Index", "Client"));
        }

        #endregion
    }
}
