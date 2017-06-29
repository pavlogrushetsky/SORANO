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

// ReSharper disable Mvc.ViewNotResolved

namespace SORANO.WEB.Controllers
{
    public class EntityBaseController<T> : BaseController where T : EntityBaseModel
    {
        protected readonly IAttachmentTypeService _attachmentTypeService;
        protected readonly IMemoryCache _memoryCache;
        protected readonly IHostingEnvironment _environment;

        /// <summary>
        /// Controller to perform entities controllers' base functionality
        /// </summary>
        /// <param name="userService">Users' service</param>
        /// <param name="environment"></param>
        /// <param name="attachmentTypeService"></param>
        /// <param name="memorycache"></param>
        public EntityBaseController(IUserService userService, IHostingEnvironment environment, IAttachmentTypeService attachmentTypeService, IMemoryCache memorycache) : base(userService)
        {
            _attachmentTypeService = attachmentTypeService;
            _memoryCache = memorycache;
            _environment = environment;
        }

        /// <summary>
        /// Post entity model to add recommendation
        /// </summary>
        /// <param name="entity">Entity model</param>
        /// <param name="isEdit">Is editing</param>
        /// <returns>Create view</returns>
        [HttpPost]
        public virtual async Task<IActionResult> AddRecommendation(T entity, bool isEdit)
        {
            ModelState.Clear();

            entity.Recommendations.Add(new RecommendationModel());

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = isEdit;

            return View("Create", entity);
        }

        /// <summary>
        /// Post entity model to remove recommendation
        /// </summary>
        /// <param name="entity">Entity model</param>
        /// <param name="isEdit">Is editing</param>
        /// <param name="num">Relative position of the recommendation</param>
        /// <returns>Create view</returns>
        [HttpPost]
        public virtual async Task<IActionResult> DeleteRecommendation(T entity, bool isEdit, int num)
        {
            ModelState.Clear();

            entity.Recommendations.RemoveAt(num);

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = isEdit;

            return View("Create", entity);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddAttachment(T entity, bool isEdit, IFormFileCollection attachments)
        {
            ModelState.Clear();

            entity.Attachments.Add(new AttachmentModel());            

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = isEdit;

            

            return View("Create", entity);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteAttachment(T entity, bool isEdit, int num)
        {
            ModelState.Clear();

            entity.Attachments.RemoveAt(num);

            ViewBag.AttachmentTypes = await GetAttachmentTypes();

            ViewData["IsEdit"] = isEdit;

            return View("Create", entity);
        }

        protected async Task<List<SelectListItem>> GetAttachmentTypes()
        {
            var attachmentTypesKey = "AttachmentTypesCache";

            if (_memoryCache.TryGetValue(attachmentTypesKey, out List<SelectListItem> attachmentTypeSelectItems))
            {
                return attachmentTypeSelectItems;
            }

            var attachmentTypes = await _attachmentTypeService.GetAllAsync();

            attachmentTypeSelectItems = attachmentTypes.Select(a => new SelectListItem
            {
                Text = a.Name,
                Value = a.ID.ToString()
            }).ToList();

            _memoryCache.Set(attachmentTypesKey, attachmentTypeSelectItems);

            return attachmentTypeSelectItems;
        }

        protected virtual async Task<string> Load(IFormFile file, string subfolder)
        {
            var extension = Path.GetExtension(file.FileName);
            var path = "/attachments/" + subfolder + "/" + Guid.NewGuid() + extension;

            using (var stream = new FileStream(_environment.WebRootPath + path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return path;            
        }
    }
}
