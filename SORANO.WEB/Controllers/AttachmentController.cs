using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels;
using SORANO.WEB.ViewModels.Attachment;
using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    [CheckUserFilter]
    public class AttachmentController : BaseController
    {
        private readonly IAttachmentService _attachmentService;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;

        public AttachmentController(IAttachmentService attachmentService, 
            IUserService userService, 
            IMemoryCache memoryCache, 
            IMapper mapper) : base(userService)
        {
            _attachmentService = attachmentService;
            _memoryCache = memoryCache;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> SelectMainPicture(int currentMainPictureId, string returnUrl)
        {
            return await TryGetActionResultAsync(async () =>
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return BadRequest();
                }

                var picturesResult = await _attachmentService.GetPicturesExceptAsync(currentMainPictureId);

                if (picturesResult.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить изображения.";
                    return Redirect(returnUrl);
                }

                var picturesModels = _mapper.Map<IEnumerable<MainPictureViewModel>>(picturesResult.Result);
                var viewModel = new SelectMainPictureViewModel
                {
                    ReturnUrl = returnUrl,
                    Pictures = picturesModels.ToList()
                };

                return View(viewModel);
            }, ex =>
            {
                TempData["Error"] = "Не удалось получить изображения.";
                return Redirect(returnUrl);
            });
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Select(SelectMainPictureViewModel model)
        {
            return TryGetActionResult(() =>
            {
                if (model.SelectedID > 0)
                {
                    if (_memoryCache.TryGetValue(CacheKeys.SelectMainPictureCacheKey, out BaseCreateUpdateViewModel cachedModel))
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
            }, ex =>
            {
                TempData["Error"] = "Не удалось выбрать изображение.";
                return Redirect(model.ReturnUrl);
            });            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(SelectMainPictureViewModel model)
        {
            Session.SetBool(CacheKeys.SelectMainPictureCacheValidKey, true);

            return Redirect(model.ReturnUrl);
        }

        #endregion
    }
}