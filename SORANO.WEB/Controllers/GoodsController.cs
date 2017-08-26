﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager,editor,user")]
    public class GoodsController : BaseController
    {
        private readonly IGoodsService _goodsService;
        private readonly ILocationService _locationService;
        private readonly IArticleService _articleService;
        private readonly IClientService _clientService;

        public GoodsController(IClientService clientService, IGoodsService goodsService, ILocationService locationService, IUserService userService, IArticleService articleService) : base(userService)
        {
            _clientService = clientService;
            _goodsService = goodsService;
            _locationService = locationService;
            _articleService = articleService;
        }

        #region GET Actions

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Sales()
        {
            var goods = await _goodsService.GetSoldGoodsAsync();

            var sales = goods.GroupBy(g => new { g.ClientID, g.DeliveryItem.ArticleID, g.SaleLocationID, g.SaleDate.Value.Date, g.SalePrice })
                    .Select(g => new SaleModel
                    {
                        ClientID = g.Key.ClientID.Value,
                        ClientName = g.First().Client.Name,
                        ArticleID = g.Key.ArticleID,
                        ArticleName = g.First().DeliveryItem.Article.Name,
                        Count = g.Count(),
                        LocationID = g.Key.SaleLocationID.Value,
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
                locationName = location?.Name;
            }

            var articleName = string.Empty;
            if (articleId.HasValue)
            {
                var article = await _articleService.GetAsync(articleId.Value);
                articleName = article?.Name;
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
                MaxCount = maxCount ?? default(int),
                Count = 1
            });
        }

        [HttpGet]
        public async Task<IActionResult> ChangeLocation(int articleId, int currentLocationId, int maxCount, string returnUrl)
        {
            var currentLocation = await _locationService.GetAsync(currentLocationId);
            var article = await _articleService.GetAsync(articleId);

            return View(new GoodsChangeLocationModel
            {
                ReturnUrl = returnUrl,
                ArticleID = articleId,
                MaxCount = maxCount,
                Count = 1,
                CurrentLocationID = currentLocationId,
                CurrentLocationName = currentLocation.Name,
                ArticleName = article.Name
            });
        }

        #endregion

        #region POST Actions

        [HttpPost]
        public async Task<IActionResult> ChangeLocation(GoodsChangeLocationModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var currentUser = await GetCurrentUser();

                await _goodsService.ChangeLocationAsync(model.ArticleID, model.CurrentLocationID, model.TargetLocationID, model.Count, currentUser.ID);

                return Redirect(model.ReturnUrl);
            }, ex =>
            {
                ModelState.AddModelError("", ex);

                return View(model);
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

                var currentUser = await GetCurrentUser();

                var parsed = decimal.TryParse(model.SalePrice, NumberStyles.Any, new CultureInfo("en-US"), out decimal salePrice);

                await _goodsService.SaleAsync(model.ArticleID, model.LocationID, model.ClientID, model.Count, salePrice, currentUser.ID);

                return Redirect(model.ReturnUrl);
            }, ex =>
            {
                ModelState.AddModelError("", ex);

                return View(model);
            });
        }

        #endregion 

        [HttpPost]
        public async Task<JsonResult> GetClients(string term)
        {
            var clients = await _clientService.GetAllAsync(false);

            return Json(new
            {
                results = clients
                    .Where(c => !string.IsNullOrEmpty(term) ? c.Name.Contains(term) : true)
                    .Select(c => new
                    {
                        id = c.ID,
                        text = c.Name
                    })
            });
        }

        [HttpPost]
        public async Task<JsonResult> GetArticles(string term, int? locationId)
        {
            var articles = await _articleService.GetArticlesForLocationAsync(locationId);

            return Json(new
            {
                results = articles
                    .Where(a => !string.IsNullOrEmpty(term) ? a.Name.Contains(term) : true)
                    .Select(a => new
                    {
                        id = a.ID,
                        text = a.Name
                    })
            });
        }

        [HttpPost]
        public async Task<JsonResult> GetLocations(string term, int? articleId)
        {
            var locations = await _locationService.GetLocationsForArticleAsync(articleId);

            return Json(new
            {
                results = locations
                    .Where(a => !string.IsNullOrEmpty(term) ? a.Name.Contains(term) : true)
                    .Select(a => new
                    {
                        id = a.ID,
                        text = a.Name
                    })
            });
        }
    }
}
