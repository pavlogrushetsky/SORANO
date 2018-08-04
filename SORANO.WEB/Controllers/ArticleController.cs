using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Dtos;
using SORANO.BLL.Services;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels.Article;
using SORANO.WEB.ViewModels.Attachment;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.WEB.ViewModels.DeliveryItem;

namespace SORANO.WEB.Controllers
{    
    [CheckUser]
    public class ArticleController : EntityBaseController<ArticleCreateUpdateViewModel>
    {
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;

        public ArticleController(IArticleService articleService,
            IUserService userService, 
            IExceptionService exceptionService,
            IHostingEnvironment environment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache, 
            IMapper mapper) : base(userService, exceptionService, environment, attachmentTypeService, attachmentService, memoryCache)
        {
            _articleService = articleService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        [Authorize(Roles = "developer,administrator,manager,user")]
        public async Task<IActionResult> Index()
        {
            return await TryGetActionResultAsync(async () =>
            {
                var showDeletedArticles = Session.GetBool("ShowDeletedArticles");
                var showDeletedArticleTypes = Session.GetBool("ShowDeletedArticleTypes");

                ViewBag.ShowDeletedArticles = showDeletedArticles;
                ViewBag.ShowDeletedArticleTypes = showDeletedArticleTypes;

                await ClearAttachments();

                return View();
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Home");
            });
        }

        [HttpGet]
        public IActionResult Table()
        {
            var showDeleted = Session.GetBool("ShowDeletedArticles");
            const ArticleTableMode mode = ArticleTableMode.ArticleIndex;
            ViewBag.ShowDeletedArticles = showDeleted;

            return ViewComponent("ArticleTable", new { showDeleted, mode });
        }

        [HttpGet]
        public IActionResult ToggleDeleted()
        {
            var showDeleted = Session.GetBool("ShowDeletedArticles");
            var toggleDeleted = !showDeleted;
            Session.SetBool("ShowDeletedArticles", toggleDeleted);
            const ArticleTableMode mode = ArticleTableMode.ArticleIndex;
            ViewBag.ShowDeletedArticles = toggleDeleted;

            return ViewComponent("ArticleTable", new { showDeleted = toggleDeleted, mode });
        }

        [HttpGet]
        [Authorize(Roles = "developer,administrator,manager,user")]
        public IActionResult ShowDeleted(bool show)
        {
            Session.SetBool("ShowDeletedArticles", show);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "developer,administrator,manager")]
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

                return View(model);
            }, OnFault);            
        }

        [HttpGet]
        [Authorize(Roles = "developer,administrator,manager")]
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

                    if (result.Status != ServiceResponseStatus.Success)
                    {
                        TempData["Error"] = "Не удалось найти указанный артикул.";
                        return RedirectToAction("Index");
                    }
                    
                    model = _mapper.Map<ArticleCreateUpdateViewModel>(result.Result);
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
                var result = await _articleService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанный артикул.";
                    return RedirectToAction("Index");
                }

                var viewModel = _mapper.Map<ArticleDetailsViewModel>(result.Result);
                viewModel.Table.Mode = DeliveryItemTableMode.ArticleDetails;

                return View(viewModel);
            }, OnFault);            
        }

        [HttpGet]
        [Authorize(Roles = "developer,administrator,manager")]
        public async Task<IActionResult> Delete(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _articleService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанный артикул.";
                    return RedirectToAction("Index");
                }               

                return View(_mapper.Map<ArticleDeleteViewModel>(result.Result));
            }, OnFault);            
        }        

        #endregion

        #region POST Actions

        [HttpPost]
        [Authorize(Roles = "developer,administrator,manager")]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public IActionResult CreateParentType(ArticleCreateUpdateViewModel model, string returnUrl, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return TryGetActionResult(() =>
            {
                MemoryCache.Set(CacheKeys.CreateArticleTypeCacheKey, model);
                return RedirectToAction("Create", "ArticleType", new {returnUrl});
            }, ex =>
            {
                TempData["Error"] = ex;
                return View("Create", model);
            });
        }

        [HttpPost]
        [Authorize(Roles = "developer,administrator,manager")]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        [ValidateModel]
        public async Task<IActionResult> Create(ArticleCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var article = _mapper.Map<ArticleDto>(model);

                var result = await _articleService.CreateAsync(article, UserId);

                switch (result.Status)
                {
                    case ServiceResponseStatus.NotFound:
                    case ServiceResponseStatus.InvalidOperation:
                        TempData["Error"] = "Не удалось создать артикул.";
                        return RedirectToAction("Index");
                    case ServiceResponseStatus.AlreadyExists:
                        ModelState.AddModelError("Barcode", "Артикул с таким штрих-кодом уже существует.");
                        return View(model);
                }

                TempData["Success"] = $"Артикул \"{model.Name}\" был успешно создан.";

                if (string.IsNullOrEmpty(model.ReturnPath))
                {
                    return RedirectToAction("Index");
                }

                if (MemoryCache.TryGetValue(CacheKeys.CreateArticleCacheKey, out DeliveryItemViewModel cachedDeliveryItem))
                {
                    cachedDeliveryItem.ArticleName = model.Name;
                    cachedDeliveryItem.ArticleID = result.Result;
                    MemoryCache.Set(CacheKeys.CreateArticleCacheKey, cachedDeliveryItem);
                    Session.SetBool(CacheKeys.CreateArticleCacheValidKey, true);
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
        public async Task<IActionResult> Update(ArticleCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var article = _mapper.Map<ArticleDto>(model);

                var result = await _articleService.UpdateAsync(article, UserId);

                switch (result.Status)
                {
                    case ServiceResponseStatus.NotFound:
                    case ServiceResponseStatus.InvalidOperation:
                        TempData["Error"] = "Не удалось обновить артикул.";
                        return RedirectToAction("Index");
                    case ServiceResponseStatus.AlreadyExists:
                        ModelState.AddModelError("Barcode", "Артикул с таким штрих-кодом уже существует.");
                        return View("Create", model);
                }

                TempData["Success"] = $"Артикул \"{model.Name}\" был успешно обновлён.";
                return RedirectToAction("Index");
            }, OnFault);
        }

        [HttpPost]
        [Authorize(Roles = "developer,administrator,manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ArticleDeleteViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _articleService.DeleteAsync(model.ID, UserId);               

                if (result.Status == ServiceResponseStatus.Success)
                {
                    TempData["Success"] = $"Артикул \"{model.Name}\" был успешно помечен как удалённый.";
                }
                else
                {
                    TempData["Error"] = "Не удалось удалить артикул.";
                }
                
                return RedirectToAction("Index");
            }, OnFault);            
        }

        [HttpPost]
        [Authorize(Roles = "developer,administrator,manager")]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(ArticleCreateUpdateViewModel model)
        {
            if (string.IsNullOrEmpty(model.ReturnPath))
            {
                return RedirectToAction("Index", "Article");
            }

            if (MemoryCache.TryGetValue(CacheKeys.CreateArticleCacheKey, out DeliveryItemViewModel _))
            {
                Session.SetBool(CacheKeys.CreateArticleCacheValidKey, true);
            }

            return Redirect(model.ReturnPath);
        }

        #endregion

        [HttpPost]
        public JsonResult GetArticles(string term)
        {
            var articles = _articleService.GetAll(false, term);

            var selectModels = _mapper.Map<IEnumerable<ArticleSelectViewModel>>(articles.Result);

            return Json(new {results = selectModels.OrderBy(s => s.Name)});
        }

        private IActionResult OnFault(string ex)
        {
            TempData["Error"] = ex;
            return RedirectToAction("Index");
        }
    }
}