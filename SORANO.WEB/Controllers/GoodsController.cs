using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels;
using SORANO.WEB.ViewModels.Goods;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;
using SORANO.BLL.Services;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.ViewModels.Recommendation;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager,editor,user")]
    [CheckUser]
    public class GoodsController : BaseController
    {
        private readonly IGoodsService _goodsService;
        private readonly ILocationService _locationService;
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;

        public GoodsController(IGoodsService goodsService, 
            ILocationService locationService, 
            IUserService userService, 
            IArticleService articleService,
            IMapper mapper) : base(userService)
        {
            _goodsService = goodsService;
            _locationService = locationService;
            _articleService = articleService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await TryGetActionResultAsync(async () =>
            {
                var goodsResult = await _goodsService.GetAllAsync();

                if (goodsResult.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить список товаров.";
                    return RedirectToAction("Index", "Home");
                }

                var goods = _mapper.Map<IEnumerable<GoodsItemViewModel>>(goodsResult.Result);
                var viewModel = new GoodsIndexViewModel
                {
                    Goods = goods.ToList()
                };

                return View(viewModel);
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Home");
            });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _goodsService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанный товар.";
                    return RedirectToAction("Index");
                }

                var viewModel = _mapper.Map<GoodsDetailsViewModel>(result.Result);

                return View(viewModel);
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Goods");
            });
        }

        [HttpGet]
        public async Task<IActionResult> AddRecommendations(string ids)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var splitted = ids.Split(',');
                var result = await _goodsService.GetAsync(Convert.ToInt32(splitted.First()));

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось загрузить указанные товары.";
                    return RedirectToAction("Index");
                }

                var viewModel = _mapper.Map<GoodsRecommendationsViewModel>(result.Result);
                viewModel.Recommendations = new List<RecommendationViewModel>();
                viewModel.Ids = ids;
                viewModel.Quantity = splitted.Length;

                return View(viewModel);
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Goods");
            });
        }

        [HttpGet]
        public async Task<IActionResult> Expand(int articleId, int articleTypeId, int locationId)
        {
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor("IsFiltered");

                var goodsResult = await _goodsService.GetAllAsync(articleId, articleTypeId, locationId, true);

                if (goodsResult.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить список товаров.";
                    return RedirectToAction("Index", "Home");
                }

                var goods = _mapper.Map<IEnumerable<GoodsItemViewModel>>(goodsResult.Result).ToList();
                var first = goods.First();
                var viewModel = new GoodsIndexViewModel
                {
                    Goods = goods.ToList(),
                    ArticleID = articleId,
                    ArticleName = first.ArticleName,
                    ArticleTypeID = articleTypeId,
                    ArticleTypeName = first.ArticleTypeName,
                    LocationID = locationId,
                    LocationName = first.LocationName,
                    ShowByPiece = true,
                    IsFiltered = true
                };

                return View("Index", viewModel);
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Goods");
            });
        }

        [HttpGet]
        public async Task<IActionResult> Sales()
        {
            var goods = await _goodsService.GetSoldGoodsAsync();

            var sales = goods.Result.GroupBy(g => new { g.ClientID, g.DeliveryItem.ArticleID, g.SaleLocationID, g.SaleDate.Value.Date, g.SalePrice })
                    .Select(g => new SaleModel
                    {
                        ClientID = g.Key.ClientID.Value,
                        ClientName = g.First().Client.Name,
                        ArticleID = g.Key.ArticleID,
                        ArticleName = g.First().DeliveryItem.Article.Name,
                        Count = g.Count(),
                        LocationID = g.Key.SaleLocationID ?? 0,//TODO
                        LocationName = g.First().SaleLocation.Name,
                        TotalPrice = (g.Count() * g.Key.SalePrice.Value).ToString("0.00") + " ₴",
                        SaleDate = g.Key.Date.ToString("dd.MM.yyyy")
                    }).ToList();

            return View(sales);
        }

        [HttpGet]
        public async Task<IActionResult> Sale(int? articleId, int? currentLocationId, int? maxCount, string returnUrl)
        {
            var locationName = string.Empty;
            if (currentLocationId.HasValue)
            {
                var location = await _locationService.GetAsync(currentLocationId.Value);
                locationName = location.Result?.Name;
            }

            var articleName = string.Empty;
            if (articleId.HasValue)
            {
                var article = await _articleService.GetAsync(articleId.Value);
                articleName = article.Result?.Name;//TODO
            }

            return View(new SaleModel
            {
                ReturnUrl = returnUrl,
                ArticleID = articleId ?? default(int),
                ArticleName = articleName,
                IsArticleEditable = !articleId.HasValue,
                LocationID = currentLocationId ?? default(int),
                LocationName = locationName,
                IsLocationEditable = !currentLocationId.HasValue,
                MaxCount = maxCount ?? 1,
                Count = 1
            });
        }

        [HttpGet]
        public async Task<IActionResult> ChangeLocation(string ids)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var splitted = ids.Split(',');
                var result = await _goodsService.GetAsync(Convert.ToInt32(splitted.First()));

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось загрузить указанные товары.";
                    return RedirectToAction("Index");
                }

                var viewModel = _mapper.Map<GoodsChangeLocationViewModel>(result.Result);
                viewModel.Ids = ids;
                viewModel.MaxCount = splitted.Length;
                viewModel.Count = viewModel.MaxCount;

                return View(viewModel);
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index", "Goods");
            });
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(GoodsIndexViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor("IsFiltered");

                var goodsResult = await _goodsService.GetAllAsync(model.ArticleID, model.ArticleTypeID, model.LocationID, model.ShowByPiece, model.Status);

                if (goodsResult.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить список товаров.";
                    return RedirectToAction("Index", "Home");
                }

                var goods = _mapper.Map<IEnumerable<GoodsItemViewModel>>(goodsResult.Result);
                var viewModel = new GoodsIndexViewModel
                {
                    Goods = goods.ToList(),
                    IsFiltered = true,
                    Status = model.Status
                };

                return View(viewModel);
            }, ex =>
            {
                TempData["Error"] = ex;
                return View(model);
            });
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetFilters(GoodsIndexViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                model.ShowByPiece = false;
                model.ShowNumber = 10;
                model.Status = 0;
                model.ArticleID = 0;
                model.ArticleTypeID = 0;
                model.IsFiltered = false;
                ModelState.Clear();

                var goodsResult = await _goodsService.GetAllAsync();

                if (goodsResult.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить список товаров.";
                    return RedirectToAction("Index", "Home");
                }

                var goods = _mapper.Map<IEnumerable<GoodsItemViewModel>>(goodsResult.Result);
                var viewModel = new GoodsIndexViewModel
                {
                    Goods = goods.ToList()
                };

                return View("Index", viewModel);
            }, ex =>
            {
                TempData["Error"] = ex;
                return View("Index", model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRecommendations(GoodsRecommendationsViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (!model.Recommendations.Any())
                    return RedirectToAction("Index");

                var recommendations = _mapper.Map<IEnumerable<RecommendationDto>>(model.Recommendations);
                var ids = model.Ids.Split(',').Select(id => Convert.ToInt32(id));
                var result = await _goodsService.AddRecommendationsAsync(ids, recommendations, UserId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось обновить рекомендации.";
                    return RedirectToAction("Index");
                }

                TempData["Success"] = "Рекомендации были успешно обновлены.";

                return RedirectToAction("Index");
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index");
            });
        }

        [HttpPost]
        public virtual IActionResult AddRecommendation(GoodsRecommendationsViewModel model)
        {
            ModelState.Clear();

            model.Recommendations.Add(new RecommendationViewModel());

            return View("AddRecommendations", model);
        }

        [HttpPost]
        public virtual IActionResult DeleteRecommendation(GoodsRecommendationsViewModel model, int num)
        {
            ModelState.Clear();

            model.Recommendations.RemoveAt(num);

            return View("AddRecommendations", model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeLocation(GoodsChangeLocationViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var ids = model.Ids.Split(',').Select(id => Convert.ToInt32(id));
                var result = await _goodsService.ChangeLocationAsync(ids, model.TargetLocationID, model.Count, UserId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось переместить товары.";
                    return RedirectToAction("Index");
                }

                TempData["Success"] = "Товары были успешно перемещены.";

                return RedirectToAction("Index");
            }, ex =>
            {
                TempData["Error"] = ex;
                return RedirectToAction("Index");
            });
        }

        [HttpPost]
        public async Task<IActionResult> Sale(SaleModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var parsed = decimal.TryParse(model.SalePrice, NumberStyles.Any, new CultureInfo("en-US"), out decimal salePrice);

                await _goodsService.SaleAsync(model.ArticleID, model.LocationID, model.ClientID, model.Count, salePrice, UserId);

                return Redirect(model.ReturnUrl);
            }, ex =>
            {
                ModelState.AddModelError("", ex);

                return View(model);
            });
        }

        #endregion 

        //[HttpPost]
        //public async Task<JsonResult> GetGoods(string term, int locationId)
        //{
        //    var articles = await _articleService.GetAllAsync(false, term);

        //    var selectModels = _mapper.Map<IEnumerable<ArticleSelectViewModel>>(articles.Result);

        //    return Json(new { results = selectModels.OrderBy(s => s.Name) });
        //}
    }
}
