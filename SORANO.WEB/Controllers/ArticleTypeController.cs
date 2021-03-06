﻿using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Dtos;
using SORANO.BLL.Services;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels.Article;
using SORANO.WEB.ViewModels.ArticleType;
using SORANO.WEB.ViewModels.Attachment;

namespace SORANO.WEB.Controllers
{    
    [CheckUser]
    public class ArticleTypeController : EntityBaseController<ArticleTypeCreateUpdateViewModel>
    {
        private readonly IArticleTypeService _articleTypeService;
        private readonly IMapper _mapper;

        public ArticleTypeController(IArticleTypeService articleTypeService, 
            IUserService userService,
            IExceptionService exceptionService,
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache, 
            IMapper mapper) : base(userService, exceptionService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _articleTypeService = articleTypeService;
            _mapper = mapper;
        }

        #region GET Actions       

        [HttpGet]
        [Authorize(Roles = "developer,administrator,manager,user")]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedArticleTypes", show);

            return RedirectToAction("Index", "Article");
        }

        [HttpGet]
        [Authorize(Roles = "developer,administrator,manager")]
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
            }, OnFault);            
        }                

        [HttpGet]
        [Authorize(Roles = "developer,administrator,manager")]
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

                    if (result.Status != ServiceResponseStatus.Success)
                    {
                        TempData["Error"] = "Не удалось найти указанный тип артикулов.";
                        return RedirectToAction("Index", "Article");
                    }

                    model = _mapper.Map<ArticleTypeCreateUpdateViewModel>(result.Result);
                    model.IsUpdate = true;
                }

                return View("Create", model);
            }, OnFault);            
        }

        [HttpGet]
        [Authorize(Roles = "developer,administrator,manager,user")]
        public async Task<IActionResult> Details(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _articleTypeService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанный тип артикулов.";
                    return RedirectToAction("Index", "Article");
                }

                var viewModel = _mapper.Map<ArticleTypeDetailsViewModel>(result.Result);
                viewModel.Articles.Mode = ArticleTableMode.ArticleTypeDetails;

                return View(viewModel);
            }, OnFault);
        }

        [HttpGet]
        [Authorize(Roles = "developer,administrator,manager")]
        public async Task<IActionResult> Delete(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _articleTypeService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанный тип артикулов.";
                    return RedirectToAction("Index", "Article");
                }
               
                return View(_mapper.Map<ArticleTypeDeleteViewModel>(result.Result));
            }, OnFault);
        }

        #endregion

        #region POST Actions

        [HttpPost]
        public IActionResult Tree(string searchTerm)
        {
            var showDeleted = Session.GetBool("ShowDeletedArticleTypes");
            ViewBag.ShowDeletedArticleTypes = showDeleted;

            return ViewComponent("ArticleTypes", new { showDeleted, searchTerm });
        }

        [HttpPost]
        public IActionResult ToggleDeleted(string searchTerm)
        {
            var showDeleted = Session.GetBool("ShowDeletedArticleTypes");
            var toggleDeleted = !showDeleted;
            Session.SetBool("ShowDeletedArticleTypes", toggleDeleted);
            ViewBag.ShowDeletedArticleTypes = toggleDeleted;

            return ViewComponent("ArticleTypes", new { showDeleted = toggleDeleted, searchTerm });
        }

        [HttpPost]
        [Authorize(Roles = "developer,administrator,manager")]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        [ValidateModel]
        public async Task<IActionResult> Create(ArticleTypeCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var articleType = _mapper.Map<ArticleTypeCreateUpdateViewModel, ArticleTypeDto>(model);

                var result = await _articleTypeService.CreateAsync(articleType, UserId);

                if (result.Status == ServiceResponseStatus.NotFound ||
                    result.Status == ServiceResponseStatus.InvalidOperation)
                {
                    TempData["Error"] = "Не удалось создать тип артикулов.";
                    return RedirectToAction("Index", "Article");
                }

                TempData["Success"] = $"Тип артикулов \"{model.Name}\" был успешно создан.";

                if (string.IsNullOrEmpty(model.ReturnPath))
                {
                    return RedirectToAction("Index", "Article");
                }

                if (MemoryCache.TryGetValue(CacheKeys.CreateArticleTypeCacheKey, out ArticleCreateUpdateViewModel cachedArticle))
                {
                    cachedArticle.TypeID = result.Result;
                    cachedArticle.TypeName = model.Name;
                    MemoryCache.Set(CacheKeys.CreateArticleTypeCacheKey, cachedArticle);
                    Session.SetBool(CacheKeys.CreateArticleTypeCacheValidKey, true);
                }
                else
                {
                    return BadRequest();
                }

                return Redirect(model.ReturnPath);
            }, OnFault);            
        }

        [HttpPost]
        [Authorize(Roles = "developer,administrator,manager")]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        [ValidateModel]
        public async Task<IActionResult> Update(ArticleTypeCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var articleType = _mapper.Map<ArticleTypeDto>(model);

                var result = await _articleTypeService.UpdateAsync(articleType, UserId);

                if (result.Status == ServiceResponseStatus.NotFound ||
                    result.Status == ServiceResponseStatus.InvalidOperation)
                {
                    TempData["Error"] = "Не удалось обновить тип артикулов.";
                    return RedirectToAction("Index", "Article");
                }

                TempData["Success"] = $"Тип артикулов \"{model.Name}\" был успешно обновлён.";
                return RedirectToAction("Index", "Article");
            }, OnFault);            
        }

        [HttpPost]
        [Authorize(Roles = "developer,administrator,manager")]
        public async Task<IActionResult> Delete(ArticleTypeDeleteViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _articleTypeService.DeleteAsync(model.ID, UserId);

                if (result.Status == ServiceResponseStatus.Success)
                {
                    TempData["Success"] = $"Тип артикулов \"{model.Name}\" был успешно помечен как удалённый.";
                }
                else
                {
                    TempData["Error"] = "Не удалось обновить тип артикулов.";
                }

                return RedirectToAction("Index", "Article");
            }, OnFault);
        }

        [HttpPost]
        [Authorize(Roles = "developer,administrator,manager")]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(ArticleTypeCreateUpdateViewModel model)
        {
            if (string.IsNullOrEmpty(model.ReturnPath))
            {
                return RedirectToAction("Index", "Article");
            }

            if (MemoryCache.TryGetValue(CacheKeys.CreateArticleTypeCacheKey, out ArticleCreateUpdateViewModel _))
            {
                Session.SetBool(CacheKeys.CreateArticleTypeCacheValidKey, true);
            }

            return Redirect(model.ReturnPath);
        }

        #endregion

        [HttpPost]
        public JsonResult GetArticleTypes(string term, int currentTypeId = 0)
        {
            var articleTypes = _articleTypeService.GetAll(false, term, currentTypeId);

            return Json(new
            {
                results = articleTypes.Result?
                    .Select(t => new
                    {
                        id = t.ID,
                        text = t.Name,
                        parent = t.Type == null ? t.Name : $"{t.Type.Name}  {'\u21d0'}  {t.Name}",
                        desc = t.Description ?? string.Empty
                    })
                    .OrderBy(t => t.parent)
            });
        }

        private IActionResult OnFault(string ex)
        {
            TempData["Error"] = ex;
            return RedirectToAction("Index", "Article");
        }
    }
}