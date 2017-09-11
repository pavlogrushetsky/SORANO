using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services.Abstract;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using MimeTypes;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.ViewModels.Attachment;
using SORANO.WEB.ViewModels.Common;
using SORANO.WEB.ViewModels.Recommendation;

// ReSharper disable Mvc.ViewNotResolved

namespace SORANO.WEB.Controllers
{
    public class EntityBaseController<T> : BaseController where T : BaseCreateUpdateViewModel
    {
        protected readonly IAttachmentTypeService AttachmentTypeService;
        protected readonly IAttachmentService AttachmentService;
        protected readonly IMemoryCache MemoryCache;
        protected readonly IHostingEnvironment Environment;
        private readonly string _entityTypeName;

        public EntityBaseController(IUserService userService, 
            IHostingEnvironment environment, 
            IAttachmentTypeService attachmentTypeService, 
            IAttachmentService attachmentService, 
            IMemoryCache memorycache) : base(userService)
        {
            AttachmentTypeService = attachmentTypeService;
            AttachmentService = attachmentService;
            MemoryCache = memorycache;
            Environment = environment;
            _entityTypeName = typeof(T).Name.ToLower().Replace("model", "");
        }

        protected bool TryGetCached(out T model, string key, string validKey)
        {
            if (MemoryCache.TryGetValue(key, out BaseCreateUpdateViewModel cachedModel))
            {
                MemoryCache.Remove(key);

                var m = cachedModel as T;
                if (m != null && Session.GetBool(validKey))
                {
                    Session.SetBool(validKey, false);
                    model = m;
                    return true;
                }
            }

            model = null;
            return false;
        }

        protected virtual async Task CopyMainPicture(T cachedModel)
        {
            var hasThisMainPicture = true;

            if (cachedModel.MainPicture.ID > 0)
            {
                hasThisMainPicture = await AttachmentService.HasMainPictureAsync(cachedModel.ID, cachedModel.MainPicture.ID, UserId);
            }
                                
            if (hasThisMainPicture)
            {
                return;
            }

            var mainPicture = cachedModel.MainPicture;

            if (string.IsNullOrEmpty(mainPicture.FullPath) || !System.IO.File.Exists(Environment.WebRootPath + mainPicture.FullPath))
            {
                return;
            }
            
            var filename = Guid.NewGuid() + Path.GetExtension(mainPicture.FullPath);
            var path = "/attachments/" + _entityTypeName + "/";
            var fullPath = Environment.WebRootPath + path;

            Directory.CreateDirectory(fullPath);

            System.IO.File.Copy(Environment.WebRootPath + mainPicture.FullPath, fullPath + filename);

            cachedModel.MainPicture = new MainPictureViewModel
            {
                TypeID = await GetMainPictureTypeID(),
                Name = mainPicture.Name,
                FullPath = path + filename
            };
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddRecommendation(T model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            ModelState.Clear();

            model.Recommendations.Add(new RecommendationViewModel());

            ViewBag.LocationTypes = GetLocationTypes();
            ViewBag.Articles = GetArticles();
            ViewBag.Suppliers = GetSuppliers();
            ViewBag.Locations = GetLocations();

            await LoadMainPicture(model, mainPictureFile);
            await LoadAttachments(model, attachments);

            return View("Create", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteRecommendation(T model, int num, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            ModelState.Clear();

            model.Recommendations.RemoveAt(num);

            ViewBag.LocationTypes = GetLocationTypes();
            ViewBag.Articles = GetArticles();
            ViewBag.Suppliers = GetSuppliers();
            ViewBag.Locations = GetLocations();

            await LoadMainPicture(model, mainPictureFile);
            await LoadAttachments(model, attachments);

            return View("Create", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddAttachment(T model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            ModelState.Clear();                        

            ViewBag.LocationTypes = GetLocationTypes();
            ViewBag.Articles = GetArticles();
            ViewBag.Suppliers = GetSuppliers();
            ViewBag.Locations = GetLocations();

            model.Attachments.Add(new AttachmentViewModel());

            await LoadMainPicture(model, mainPictureFile);
            await LoadAttachments(model, attachments);

            return View("Create", model);
        }

        [HttpPost]
        public virtual async Task<string> GetMimeType(int id)
        {
            var type = await AttachmentTypeService.GetAsync(id, UserId);

            return type.ToModel().MimeTypes;
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteAttachment(T entity, int num, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            ModelState.Clear();

            entity.Attachments.RemoveAt(num);

            ViewBag.LocationTypes = GetLocationTypes();
            ViewBag.Articles = GetArticles();
            ViewBag.Suppliers = GetSuppliers();
            ViewBag.Locations = GetLocations();

            await LoadMainPicture(entity, mainPictureFile);
            await LoadAttachments(entity, attachments);

            return View("Create", entity);
        }

        [HttpPost]
        public async Task<IActionResult> SelectMainPicture(T model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            await LoadMainPicture(model, mainPictureFile);
            await LoadAttachments(model, attachments);

            MemoryCache.Set(CacheKeys.SelectMainPictureCacheKey, model);

            return RedirectToAction("SelectMainPicture", "Attachment", new { currentMainPictureId = model.MainPicture.ID, returnUrl });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteMainPicture(T entity, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            ModelState.Clear();

            entity.MainPicture = new MainPictureViewModel();

            ViewBag.LocationTypes = GetLocationTypes();
            ViewBag.Articles = GetArticles();
            ViewBag.Suppliers = GetSuppliers();
            ViewBag.Locations = GetLocations();

            await LoadMainPicture(entity, mainPictureFile);
            await LoadAttachments(entity, attachments);

            return View("Create", entity);
        }

        public virtual FileResult DownloadFile(string path, string name)
        {
            var bytes = System.IO.File.ReadAllBytes(Environment.WebRootPath + path);

            return File(bytes, MimeTypeMap.GetMimeType(Path.GetExtension(path)), name);
        }

        private List<SelectListItem> GetLocationTypes()
        {
            if (MemoryCache.TryGetValue(CacheKeys.LocationTypesCacheKey, out List<SelectListItem> locationTypeSelectItems))
            {
                return locationTypeSelectItems;
            }

            return null;
        }

        private List<SelectListItem> GetSuppliers()
        {
            if (MemoryCache.TryGetValue(CacheKeys.SuppliersCacheKey, out List<SelectListItem> supplierSelectItems))
            {
                return supplierSelectItems;
            }

            return null;
        }

        private List<SelectListItem> GetLocations()
        {
            if (MemoryCache.TryGetValue(CacheKeys.LocationsCacheKey, out List<SelectListItem> locationSelectItems))
            {
                return locationSelectItems;
            }

            return null;
        }

        private List<SelectListItem> GetArticles()
        {
            if (MemoryCache.TryGetValue(CacheKeys.ArticlesCacheKey, out List<SelectListItem> articleCodesSelectItems))
            {
                return articleCodesSelectItems;
            }

            return null;
        }

        [HttpPost]
        public async Task<JsonResult> GetAttachmentTypes(string term)
        {
            var attachmentTypes = await AttachmentTypeService.GetAllAsync(false, UserId);

            return Json(new
            {
                results = attachmentTypes
                    .Where(t => string.IsNullOrEmpty(term) || t.Name.Contains(term))
                    .Select(t => new
                    {
                        id = t.ID,
                        text = t.Name
                    })
            });
        }

        protected virtual async Task<int> GetMainPictureTypeID()
        {
            var result = await AttachmentTypeService.GetMainPictureTypeIdAsync(UserId);

            return result.Result;
        }

        protected virtual async Task<string> Load(IFormFile file, string subfolder)
        {
            var extension = Path.GetExtension(file.FileName);
            var filename = Guid.NewGuid() + extension;
            var path = "/attachments/" + subfolder + "/";
            var fullPath = Environment.WebRootPath + path;

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
                        model.Attachments[i].IsNew = false;

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

                ModelState.RemoveFor("MainPicture");
            }
        }

        protected virtual async Task ClearAttachments()
        {
            var path = Environment.WebRootPath + "/attachments/" + _entityTypeName + "/";
            if (!Directory.Exists(path))
            {
                return;
            }

            var files = Directory.GetFiles(path);
            var attachments = await AttachmentService.GetAllForAsync(_entityTypeName, UserId);
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
    }
}
