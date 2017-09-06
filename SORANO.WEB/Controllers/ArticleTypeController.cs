using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.ViewModels;
using SORANO.WEB.ViewModels.ArticleType;
using SORANO.WEB.ViewModels.Attachment;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class ArticleTypeController : EntityBaseController<ArticleTypeCreateUpdateViewModel>
    {
        private readonly IArticleTypeService _articleTypeService;
        private readonly IMapper _mapper;

        public ArticleTypeController(IArticleTypeService articleTypeService, 
            IUserService userService,
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache, 
            IMapper mapper) : base(userService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _articleTypeService = articleTypeService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedArticleTypes", show);

            return RedirectToAction("Index", "Article");
        }

        [HttpGet]
        public async Task<IActionResult> Create(string returnUrl)
        {
            return await TryGetActionResultAsync(async () =>
            {
                ArticleTypeCreateUpdateViewModel model;

                if (TryGetCached(out var cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
                {
                    model = cachedForSelectMainPicture;
                    await CopyMainPicture(model);
                }
                else
                {
                    model = new ArticleTypeCreateUpdateViewModel
                    {
                        MainPicture = new MainPictureViewModel(),
                        ReturnPath = returnUrl
                    };
                }

                return View(model);
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Article");
            });            
        }

        [HttpGet]
        public async Task<IActionResult> Brief(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _articleTypeService.GetAsync(id);

                if (result.Status == ServiceResponseStatusType.Fail)
                {
                    TempData["Error"] = result.Message;
                    return RedirectToAction("Index", "Article");
                }

                await ClearAttachments();

                return PartialView("_Brief", _mapper.Map<ArticleTypeBriefViewModel>(result.Result));
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Article");
            });           
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                ArticleTypeCreateUpdateViewModel model;

                if (TryGetCached(out var cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedForSelectMainPicture.ID == id)
                {
                    model = cachedForSelectMainPicture;
                    await CopyMainPicture(model);
                }
                else
                {
                    var result = await _articleTypeService.GetAsync(id);

                    if (result.Status == ServiceResponseStatusType.Fail)
                    {
                        TempData["Error"] = result.Message;
                        return RedirectToAction("Index", "Article");
                    }

                    model = _mapper.Map<ArticleTypeCreateUpdateViewModel>(result.Result);
                    model.IsUpdate = true;
                }

                return View("Create", model);
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Article");
            });            
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _articleTypeService.GetAsync(id);

                if (result.Status != ServiceResponseStatusType.Fail)
                {
                    return View(_mapper.Map<ArticleTypeDeleteViewModel>(result.Result));
                }

                TempData["Error"] = result.Message;
                return RedirectToAction("Index", "Article");
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Article");
            });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _articleTypeService.GetAsync(id);

                if (result.Status != ServiceResponseStatusType.Fail)
                {
                    return View(_mapper.Map<ArticleTypeDetailsViewModel>(result.Result));
                }

                TempData["Error"] = result.Message;
                return RedirectToAction("Index", "Article");
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Article");
            });           
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleTypeModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var articleType = model.ToEntity();

                var currentUser = await GetCurrentUser();

                articleType = await _articleTypeService.CreateAsync(articleType, currentUser.ID);

                if (articleType != null)
                {
                    if (string.IsNullOrEmpty(model.ReturnPath))
                    {
                        return RedirectToAction("Index", "Article");
                    }

                    if (MemoryCache.TryGetValue(CacheKeys.CreateArticleTypeCacheKey, out ArticleModel cachedArticle))
                    {
                        cachedArticle.TypeID = articleType.ID.ToString();
                        cachedArticle.Type = articleType.ToModel();
                        MemoryCache.Set(CacheKeys.CreateArticleTypeCacheKey, cachedArticle);
                        Session.SetBool(CacheKeys.CreateArticleTypeCacheValidKey, true);
                    }
                    else
                    {
                        return BadRequest();
                    }

                    return Redirect(model.ReturnPath);
                }

                ModelState.AddModelError("", "Не удалось создать новый тип артикулов.");
                return View(model);
            }, ex => 
            {
                ModelState.AddModelError("", ex);
                return View(model);
            });            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ArticleTypeModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    return View("Create", model);
                }

                var articleType = model.ToEntity();

                var currentUser = await GetCurrentUser();

                articleType = await _articleTypeService.UpdateAsync(articleType, currentUser.ID);

                if (articleType != null)
                {
                    return RedirectToAction("Index", "Article");
                }

                ModelState.AddModelError("", "Не удалось обновить тип артикулов.");
                ViewData["IsEdit"] = true;
                return View("Create", model);
            }, ex => 
            {
                ModelState.AddModelError("", ex);
                ViewData["IsEdit"] = true;
                return View("Create", model);
            });            
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ArticleTypeModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var currentUser = await GetCurrentUser();

                await _articleTypeService.DeleteAsync(model.ID, currentUser.ID);

                return RedirectToAction("Index", "Article");
            }, ex => RedirectToAction("Index", "Article"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(ArticleTypeModel model)
        {
            if (string.IsNullOrEmpty(model.ReturnPath))
            {
                return RedirectToAction("Index", "Article");
            }

            if (MemoryCache.TryGetValue(CacheKeys.CreateArticleTypeCacheKey, out ArticleModel _))
            {
                Session.SetBool(CacheKeys.CreateArticleTypeCacheValidKey, true);
            }

            return Redirect(model.ReturnPath);
        }

        #endregion

        [HttpPost]
        public async Task<JsonResult> GetArticleTypes(string term, int currentTypeId = 0)
        {
            var articleTypes = await _articleTypeService.GetAllAsync(false);

            return Json(new
            {
                results = articleTypes.Result?
                    .Where(t => (string.IsNullOrEmpty(term) || t.Name.Contains(term)) && t.ID != currentTypeId)
                    .Select(t => new
                    {
                        id = t.ID,
                        text = t.Name
                    })
            });
        }
    }
}