using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels.Goods;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;
using SORANO.BLL.Services;
using SORANO.WEB.ViewModels.Recommendation;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager,editor,user")]
    [CheckUser]
    public class GoodsController : BaseController
    {
        private readonly IGoodsService _goodsService;
        private readonly IMapper _mapper;

        public GoodsController(IGoodsService goodsService, 
            IExceptionService exceptionService,
            ILocationService locationService, 
            IUserService userService, 
            IArticleService articleService,
            IMapper mapper) : base(userService, exceptionService)
        {
            _goodsService = goodsService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new GoodsIndexViewModel();

            if (!LocationId.HasValue)
            {
                viewModel.AllowChangeLocation = true;
                return View(viewModel);
            }                

            viewModel.LocationID = LocationId.Value;
            viewModel.LocationName = LocationName;

            return View(viewModel);
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
        public IActionResult Filter(GoodsIndexViewModel model)
        {
            return ViewComponent("Goods", new {model});
        }

        [HttpGet]
        public IActionResult ClearFilter()
        {
            var model = new GoodsIndexViewModel();
            if (LocationId.HasValue)
                model.LocationID = LocationId.Value;

            return ViewComponent("Goods", new { model });
        }

        [HttpPost]
        public IActionResult Expand(GoodsIndexViewModel model)
        {
            return ViewComponent("Goods", new { model });
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(GoodsIndexViewModel model, string goods, int saleId)
        {
            var splitted = goods.Split(',');
            var ids = splitted.Select(s => Convert.ToInt32(s));

            await _goodsService.AddToCartAsync(ids, saleId, UserId);

            return ViewComponent("Goods", new { model });
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Index(GoodsIndexViewModel model)
        //{
        //    return await TryGetActionResultAsync(async () =>
        //    {
        //        ModelState.RemoveFor("IsFiltered");

        //        var goodsResult = await _goodsService.GetAllAsync(model.ArticleID, model.ArticleTypeID, model.LocationID, model.ShowByPiece, model.Status);

        //        if (goodsResult.Status != ServiceResponseStatus.Success)
        //        {
        //            TempData["Error"] = "Не удалось получить список товаров.";
        //            return RedirectToAction("Index", "Home");
        //        }

        //        var goods = _mapper.Map<IEnumerable<GoodsItemViewModel>>(goodsResult.Result);
        //        var viewModel = new GoodsIndexViewModel
        //        {
        //            Goods = goods.ToList(),
        //            IsFiltered = true,
        //            Status = model.Status
        //        };

        //        return View(viewModel);
        //    }, ex =>
        //    {
        //        TempData["Error"] = ex;
        //        return View(model);
        //    });
        //}        

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ResetFilters(GoodsIndexViewModel model)
        //{
        //    return await TryGetActionResultAsync(async () =>
        //    {
        //        model.ShowByPiece = false;
        //        model.ShowNumber = 10;
        //        model.Status = 0;
        //        model.ArticleID = 0;
        //        model.ArticleTypeID = 0;
        //        model.IsFiltered = false;
        //        ModelState.Clear();

        //        var goodsResult = await _goodsService.GetAllAsync();

        //        if (goodsResult.Status != ServiceResponseStatus.Success)
        //        {
        //            TempData["Error"] = "Не удалось получить список товаров.";
        //            return RedirectToAction("Index", "Home");
        //        }

        //        var goods = _mapper.Map<IEnumerable<GoodsItemViewModel>>(goodsResult.Result);
        //        var viewModel = new GoodsIndexViewModel
        //        {
        //            Goods = goods.ToList()
        //        };

        //        return View("Index", viewModel);
        //    }, ex =>
        //    {
        //        TempData["Error"] = ex;
        //        return View("Index", model);
        //    });
        //}

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

        #endregion 
    }
}
