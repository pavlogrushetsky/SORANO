using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Models;
using SORANO.WEB.Models.Attachment;
using SORANO.WEB.Models.Recommendation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using MimeTypes;
using SORANO.WEB.Infrastructure.Extensions;

// ReSharper disable Mvc.ViewNotResolved

namespace SORANO.WEB.Controllers
{
    public class EntityBaseController<T> : BaseController where T : EntityBaseModel
    {
        protected readonly IAttachmentTypeService _attachmentTypeService;
        protected readonly IAttachmentService _attachmentService;
        protected readonly IMemoryCache _memoryCache;
        protected readonly IHostingEnvironment _environment;
        private readonly string _attachmentTypesKey = "AttachmentTypesCache";       
        private readonly string _entityTypeName;

        /// <summary>
        /// Controller to perform entities controllers' base functionality
        /// </summary>
        /// <param name="userService">Users' service</param>
        /// <param name="environment"></param>
        /// <param name="attachmentTypeService"></param>
        /// <param name="attachmentService"></param>
        /// <param name="memorycache"></param>
        public EntityBaseController(IUserService userService, IHostingEnvironment environment, IAttachmentTypeService attachmentTypeService, IAttachmentService attachmentService, IMemoryCache memorycache) : base(userService)
        {
            _attachmentTypeService = attachmentTypeService;
            _attachmentService = attachmentService;
            _memoryCache = memorycache;
            _environment = environment;
            _entityTypeName = typeof(T).Name.ToLower().Replace("model", "");
        }        

        /// <summary>
        /// Tries to get cached model from memory cache
        /// </summary>
        /// <param name="model">Cached model</param>
        /// <returns>True if succeeded</returns>
        protected bool TryGetCached(out T model)
        {
            if (_memoryCache.TryGetValue(_cachedModelKey, out EntityBaseModel cachedModel))
            {
                _memoryCache.Remove(_cachedModelKey);

                if (cachedModel is T && Session.GetBool(_isCachedModelValid))
                {
                    Session.SetBool(_isCachedModelValid, false);
                    model = cachedModel as T;
                    return true;
                }
            }

            model = null;
            return false;
        }

        /// <summary>
        /// Post entity model to add recommendation
        /// </summary>
        /// <param name="entity">Entity model</param>
        /// <param name="isEdit">Is editing</param>
        /// <param name="mainPictureFile"></param>
        /// <param name="attachments"></param>
        /// <returns>Create view</returns>
        [HttpPost]
        public virtual async Task<IActionResult> AddRecommendation(T entity, bool isEdit, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            ModelState.Clear();

            entity.Recommendations.Add(new RecommendationModel());

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = isEdit;

            await LoadMainPicture(entity, mainPictureFile);
            await LoadAttachments(entity, attachments);

            return View("Create", entity);
        }

        /// <summary>
        /// Post entity model to remove recommendation
        /// </summary>
        /// <param name="entity">Entity model</param>
        /// <param name="isEdit">Is editing</param>
        /// <param name="num">Relative position of the recommendation</param>
        /// <param name="mainPictureFile"></param>
        /// <param name="attachments"></param>
        /// <returns>Create view</returns>
        [HttpPost]
        public virtual async Task<IActionResult> DeleteRecommendation(T entity, bool isEdit, int num, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            ModelState.Clear();

            entity.Recommendations.RemoveAt(num);

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = isEdit;

            await LoadMainPicture(entity, mainPictureFile);
            await LoadAttachments(entity, attachments);

            return View("Create", entity);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddAttachment(T entity, bool isEdit, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            ModelState.Clear();                        

            var attachmentTypes = await GetAttachmentTypes();
            attachmentTypes[0].Selected = true;
            ViewBag.AttachmentTypes = attachmentTypes;

            var defaultType = await _attachmentTypeService.GetAsync(int.Parse(attachmentTypes[0].Value));

            entity.Attachments.Add(new AttachmentModel
            {
                Type = defaultType.ToModel(),
                TypeID = defaultType.ID.ToString()
            });

            ViewData["IsEdit"] = isEdit;

            await LoadMainPicture(entity, mainPictureFile);
            await LoadAttachments(entity, attachments);

            return View("Create", entity);
        }

        [HttpPost]
        public virtual async Task<string> GetMimeType(int id)
        {
            var type = await _attachmentTypeService.GetAsync(id);

            return type.ToModel().MimeTypes;
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteAttachment(T entity, bool isEdit, int num, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            ModelState.Clear();

            entity.Attachments.RemoveAt(num);

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = isEdit;

            await LoadMainPicture(entity, mainPictureFile);
            await LoadAttachments(entity, attachments);

            return View("Create", entity);
        }

        [HttpPost]
        public async Task<IActionResult> SelectMainPicture(T model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            await LoadMainPicture(model, mainPictureFile);
            await LoadAttachments(model, attachments);

            _memoryCache.Set(_cachedModelKey, model);

            return RedirectToAction("SelectMainPicture", "Attachment", new { model.ID, returnUrl });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteMainPicture(T entity, bool isEdit, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            ModelState.Clear();

            entity.MainPicture = new AttachmentModel();

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = isEdit;

            await LoadMainPicture(entity, mainPictureFile);
            await LoadAttachments(entity, attachments);

            return View("Create", entity);
        }

        public virtual FileResult DownloadFile(string path, string name)
        {
            var bytes = System.IO.File.ReadAllBytes(_environment.WebRootPath + path);

            return File(bytes, MimeTypeMap.GetMimeType(Path.GetExtension(path)), name);
        }

        protected virtual async Task<List<SelectListItem>> GetAttachmentTypes()
        {
            if (_memoryCache.TryGetValue(_attachmentTypesKey, out List<SelectListItem> attachmentTypeSelectItems))
            {
                return attachmentTypeSelectItems;
            }

            return await CacheAttachmentTypes();
        }

        protected virtual async Task<List<SelectListItem>> CacheAttachmentTypes()
        {
            var attachmentTypes = await _attachmentTypeService.GetAllAsync();

            var attachmentTypeSelectItems = attachmentTypes.Where(t => !t.Name.Equals("Основное изображение")).Select(a => new SelectListItem
            {
                Text = a.Name,
                Value = a.ID.ToString()
            }).ToList();

            _memoryCache.Set(_attachmentTypesKey, attachmentTypeSelectItems);

            return attachmentTypeSelectItems;
        }

        protected virtual async Task<string> GetMainPictureTypeID()
        {
            var id = await _attachmentTypeService.GetMainPictureTypeIDAsync();

            return id.ToString();
        }

        protected virtual async Task<string> Load(IFormFile file, string subfolder)
        {
            var extension = Path.GetExtension(file.FileName);
            var filename = Guid.NewGuid() + extension;
            var path = "/attachments/" + subfolder + "/";
            var fullPath = _environment.WebRootPath + path;

            Directory.CreateDirectory(fullPath);

            using (var stream = new FileStream(fullPath + filename, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return path + filename;            
        }

        protected virtual async Task LoadAttachments(T model, IFormFileCollection attachments)
        {
            var attachmentCounter = 0;

            if (attachments.Any())
            {
                for (var i = 0; i < model.Attachments.Count; i++)
                {
                    if (!model.Attachments[i].IsNew)
                    {
                        continue;
                    }

                    var newAttachment = attachments.Skip(attachmentCounter).Take(1).FirstOrDefault();
                    if (newAttachment != null)
                    {
                        var path = await Load(newAttachment, _entityTypeName);
                        model.Attachments[i].FullPath = path;

                        ModelState.RemoveFor($"Attachments[{i}].FullPath");                       
                    }

                    attachmentCounter++;
                }
            }
        }

        protected virtual async Task LoadMainPicture(T model, IFormFile mainPicture)
        {
            if (mainPicture != null)
            {
                var path = await Load(mainPicture, _entityTypeName);

                model.MainPicture.TypeID = await GetMainPictureTypeID();
                model.MainPicture.FullPath = path;
                model.MainPicture.Name = mainPicture.FileName;
            }
        }

        protected virtual async Task ClearAttachments()
        {
            var path = _environment.WebRootPath + "/attachments/" + _entityTypeName + "/";
            if (!Directory.Exists(path))
            {
                return;
            }

            var files = Directory.GetFiles(path);
            var attachments = await _attachmentService.GetAllForAsync(_entityTypeName);
            var fileNames = attachments.Select(Path.GetFileName).ToList();

            foreach (var file in files)
            {
                if (fileNames.Contains(Path.GetFileName(file)))
                {
                    continue;
                }

                try
                {
                    System.IO.File.Delete(file);
                }
                catch
                {
                    // ignored
                }
            }
        }

        protected virtual T CopyMainPicture(T cachedModel)
        {
            var mainPicture = cachedModel.MainPicture;

            if (string.IsNullOrEmpty(mainPicture?.FullPath) || System.IO.File.Exists(mainPicture.FullPath))
            {
                return cachedModel;
            }

            var filename = Guid.NewGuid() + Path.GetExtension(mainPicture.FullPath);
            var fullPath = _environment.WebRootPath + "/attachments/" + _entityTypeName + "/";

            Directory.CreateDirectory(fullPath);
            
            System.IO.File.Copy(mainPicture.FullPath, fullPath + filename);

            cachedModel.MainPicture = new AttachmentModel
            {
                TypeID = mainPicture.TypeID,
                Name = mainPicture.Name,
                FullPath = fullPath + filename
            };

            return cachedModel;
        }
    }
}
