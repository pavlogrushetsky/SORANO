using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
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
        public async Task<IActionResult> SelectMainPicture(int id, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                return BadRequest();
            }

            var attachments = await _attachmentService.GetPicturesForAsync(id);

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
                if (_memoryCache.TryGetValue("ForMainPictureSelection", out EntityBaseModel cachedModel))
                {
                    cachedModel.MainPicture = model.Pictures.Single(p => p.ID == model.SelectedID);
                    _memoryCache.Set("ForMainPictureSelection", cachedModel);
                    _memoryCache.Set("MainPictureSelected", true);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                _memoryCache.Set("MainPictureSelected", true);
            }

            return Redirect(model.ReturnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(SelectMainPictureModel model)
        {
            _memoryCache.Set("MainPictureSelected", true);

            return Redirect(model.ReturnUrl);
        }
    }
}