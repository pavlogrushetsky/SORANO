using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Models;
using SORANO.WEB.Models.Attachment;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class AttachmentController : BaseController
    {
        private readonly IAttachmentService _attachmentService;
        private readonly IMemoryCache _memoryCache;

        public AttachmentController(IAttachmentService attachmentService, IUserService userService, IMemoryCache memoryCache) : base(userService)
        {
            _attachmentService = attachmentService;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<IActionResult> SelectMainPicture(int currentMainPictureId, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                return BadRequest();
            }

            var attachments = await _attachmentService.GetPicturesExceptAsync(currentMainPictureId);

            var model = new SelectMainPictureModel
            {
                ReturnUrl = returnUrl,
                Pictures = attachments.Select(a => a.ToModel()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Select(SelectMainPictureModel model)
        {
            if (model.SelectedID > 0)
            {
                if (_memoryCache.TryGetValue(CacheKeys.SelectMainPictureCacheKey, out EntityBaseModel cachedModel))
                {
                    cachedModel.MainPicture = model.Pictures.Single(p => p.ID == model.SelectedID);
                    _memoryCache.Set(CacheKeys.SelectMainPictureCacheKey, cachedModel);
                    Session.SetBool(CacheKeys.SelectMainPictureCacheValidKey, true);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                Session.SetBool(CacheKeys.SelectMainPictureCacheValidKey, true);
            }

            return Redirect(model.ReturnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(SelectMainPictureModel model)
        {
            Session.SetBool(CacheKeys.SelectMainPictureCacheValidKey, true);

            return Redirect(model.ReturnUrl);
        }
    }
}