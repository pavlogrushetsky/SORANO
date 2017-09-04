﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.DTOs;
using SORANO.BLL.Services;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.ViewModels;
using SORANO.WEB.ViewModels.Article;
using SORANO.WEB.ViewModels.Attachment;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class ArticleController : EntityBaseController<ArticleCreateUpdateViewModel>
    {
        private readonly IArticleService _articleService;
        private readonly IArticleTypeService _articleTypeService;
        private readonly IMapper _mapper;

        public ArticleController(IArticleService articleService,
            IArticleTypeService articleTypeService,
            IUserService userService, 
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache, 
            IMapper mapper) : base(userService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _articleService = articleService;
            _articleTypeService = articleTypeService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await TryGetActionResultAsync(async () =>
            {
                var showDeletedArticles = Session.GetBool("ShowDeletedArticles");
                var showDeletedArticleTypes = Session.GetBool("ShowDeletedArticleTypes");

                var result = await _articleService.GetAllAsync(showDeletedArticles);

                if (result.Status == ServiceResponseStatusType.Fail)
                {
                    TempData["Error"] = result.Message;
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ShowDeletedArticles = showDeletedArticles;
                ViewBag.ShowDeletedArticleTypes = showDeletedArticleTypes;

                await ClearAttachments();

                var viewModel = _mapper.Map<IEnumerable<ArticleIndexViewModel>>(result.Result);

                return View(viewModel);
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Home");
            });
        }

        [HttpGet]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedArticles", show);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create(string returnUrl)
        {
            return await TryGetActionResultAsync(async () =>
            {
                ArticleCreateUpdateViewModel model;

                if (TryGetCached(out var cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey))
                {
                    model = cachedForSelectMainPicture;
                    await CopyMainPicture(model);
                }
                else if (TryGetCached(out var cachedForCreateType, CacheKeys.CreateArticleTypeCacheKey, CacheKeys.CreateArticleTypeCacheValidKey))
                {
                    model = cachedForCreateType;
                }
                else
                {
                    model = new ArticleCreateUpdateViewModel
                    {
                        MainPicture = new MainPictureViewModel(),
                        ReturnPath = returnUrl
                    };
                }

                ViewBag.AttachmentTypes = await GetAttachmentTypes();

                return View(model);
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index");
            });            
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                ArticleCreateUpdateViewModel model;

                if (TryGetCached(out var cachedForSelectMainPicture, CacheKeys.SelectMainPictureCacheKey, CacheKeys.SelectMainPictureCacheValidKey) && cachedForSelectMainPicture.ID == id)
                {
                    model = cachedForSelectMainPicture;
                    await CopyMainPicture(model);
                }
                else if (TryGetCached(out var cachedForCreateType, CacheKeys.CreateArticleTypeCacheKey, CacheKeys.CreateArticleTypeCacheValidKey) && cachedForCreateType.ID == id)
                {
                    model = cachedForCreateType;
                }
                else
                {
                    var result = await _articleService.GetAsync(id);

                    if (result.Status == ServiceResponseStatusType.Fail)
                    {
                        TempData["Error"] = result.Message;
                        return RedirectToAction("Index");
                    }
                    
                    model = _mapper.Map<ArticleCreateUpdateViewModel>(result.Result);
                    model.IsUpdate = true;
                }

                ViewBag.AttachmentTypes = await GetAttachmentTypes();

                return View("Create", model);
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index");
            });            
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _articleService.GetAsync(id);

                if (result.Status != ServiceResponseStatusType.Fail)
                    return View(/*result.Result.ToModel()*/);//todo

                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index");
            });            
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _articleService.GetAsync(id);

                if (result.Status != ServiceResponseStatusType.Fail)
                    return View(_mapper.Map<ArticleDeleteViewModel>(result.Result));

                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index");
            });            
        }        

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateParentType(ArticleCreateUpdateViewModel model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            await LoadMainPicture(model, mainPictureFile);
            await LoadAttachments(model, attachments);

            MemoryCache.Set(CacheKeys.CreateArticleTypeCacheKey, model);

            return RedirectToAction("Create", "ArticleType", new { returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();

            return await TryGetActionResultAsync(async () =>
            {
                attachmentTypes = await GetAttachmentTypes();

                ViewBag.AttachmentTypes = attachmentTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                var barcodeExists = await _articleService.BarcodeExistsAsync(model.Barcode);

                if (barcodeExists.Status == ServiceResponseStatusType.Fail)
                {
                    TempData["Error"] = barcodeExists.Message;
                    return RedirectToAction("Index");
                }

                if (barcodeExists.Result)
                {
                    ModelState.AddModelError("Barcode", "Артикул с таким штрих-кодом уже существует");
                }

                if (!ModelState.IsValid)
                {                   
                    return View(model);
                }

                var article = _mapper.Map<ArticleDto>(model);

                var currentUser = await GetCurrentUser();

                var result = await _articleService.CreateAsync(article, currentUser.ID);

                if (result.Status == ServiceResponseStatusType.Success && result.Result != null)
                {
                    if (string.IsNullOrEmpty(model.ReturnPath))
                    {
                        TempData["Success"] = $"Артикул \"{model.Name}\" был успешно создан";
                        return RedirectToAction("Index");
                    }

                    if (MemoryCache.TryGetValue(CacheKeys.CreateArticleCacheKey, out DeliveryModel cachedDelivery))
                    {
                        cachedDelivery.DeliveryItems[cachedDelivery.CurrentItemNumber].Article = model;
                        cachedDelivery.DeliveryItems[cachedDelivery.CurrentItemNumber].ArticleID = article.ID.ToString();
                        MemoryCache.Set(CacheKeys.CreateArticleCacheKey, cachedDelivery);
                        Session.SetBool(CacheKeys.CreateArticleCacheValidKey, true);
                    }
                    else
                    {
                        return BadRequest();
                    }

                    return Redirect(model.ReturnPath);
                }

                ViewBag.Error = result.Message;
                return View(model);
            }, ex =>
            {
                ViewBag.AttachmentTypes = attachmentTypes;

                TempData["Error"] = ex;
                return RedirectToAction("Index");
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ArticleCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            var attachmentTypes = new List<SelectListItem>();

            return await TryGetActionResultAsync(async () =>
            {
                attachmentTypes = await GetAttachmentTypes();

                ViewBag.AttachmentTypes = attachmentTypes;

                await LoadMainPicture(model, mainPictureFile);
                await LoadAttachments(model, attachments);

                var barcodeExists = await _articleService.BarcodeExistsAsync(model.Barcode, model.ID);

                if (barcodeExists.Status == ServiceResponseStatusType.Fail)
                {
                    TempData["Error"] = barcodeExists.Message;
                    return RedirectToAction("Index");
                }

                if (barcodeExists.Result)
                {
                    ModelState.AddModelError("Barcode", "Артикул с таким штрих-кодом уже существует");
                }

                if (!ModelState.IsValid)
                {
                    return View("Create", model);
                }

                var article = _mapper.Map<ArticleDto>(model);

                var currentUser = await GetCurrentUser();

                var result = await _articleService.UpdateAsync(article, currentUser.ID);

                if (result.Status == ServiceResponseStatusType.Success)
                {
                    TempData["Success"] = $"Артикул \"{model.Name}\" был успешно обновлён";
                    return RedirectToAction("Index", "Article");
                }

                TempData["Error"] = result.Message;
                return View("Create", model);
            }, ex =>
            {
                ViewBag.AttachmentTypes = attachmentTypes;

                TempData["Error"] = ex;
                return RedirectToAction("Index");
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ArticleDeleteViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var currentUser = await GetCurrentUser();

                var result = await _articleService.DeleteAsync(model.ID, currentUser.ID);

                if (result.Status == ServiceResponseStatusType.Success)
                {
                    TempData["Success"] = $"Артикул \"{model.Name}\" был успешно помечен как удалённый";
                }
                else
                {
                    TempData["Error"] = result.Message;
                }
                
                return RedirectToAction("Index");
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index");
            });            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(ArticleModel model)
        {
            if (string.IsNullOrEmpty(model.ReturnPath))
            {
                return RedirectToAction("Index", "Article");
            }

            if (MemoryCache.TryGetValue(CacheKeys.CreateArticleCacheKey, out DeliveryModel _))
            {
                Session.SetBool(CacheKeys.CreateArticleCacheValidKey, true);
            }

            return Redirect(model.ReturnPath);
        }

        #endregion

        [HttpPost]
        public async Task<JsonResult> GetArticleTypes(string term)
        {
            var articleTypes = await _articleTypeService.GetAllAsync(false);

            return Json(new
            {
                results = articleTypes
                    .Where(c => string.IsNullOrEmpty(term) || c.Name.Contains(term))
                    .Select(c => new
                    {
                        id = c.ID,
                        text = c.Name
                    })
            });
        }
    }
}