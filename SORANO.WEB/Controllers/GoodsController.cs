using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Models;
using System.Collections.Generic;
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

        public GoodsController(IGoodsService goodsService, ILocationService locationService, IUserService userService, IArticleService articleService) : base(userService)
        {
            _goodsService = goodsService;
            _locationService = locationService;
            _articleService = articleService;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> ChangeLocation(int articleId, int currentLocationId, int maxCount, string returnUrl)
        {
            ViewBag.Locations = await GetLocations(currentLocationId);

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
            var locations = new List<SelectListItem>();

            return await TryGetActionResultAsync(async () =>
            {
                locations = await GetLocations(model.CurrentLocationID);

                ViewBag.Locations = locations;

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var currentUser = await GetCurrentUser();

                await _goodsService.ChangeLocationAsync(model.ArticleID, model.CurrentLocationID, model.TargetLocationID, model.Count, currentUser.ID);

                return Redirect(model.ReturnUrl);
            }, ex =>
            {
                ViewBag.Locations = locations;

                ModelState.AddModelError("", ex);

                return View(model);
            });
        }

        #endregion 

        private async Task<List<SelectListItem>> GetLocations(int exceptId)
        {
            var locations = await _locationService.GetAllAsync(false);

            var locationItems = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "0",
                    Text = "-- Место --"
                }
            };

            locationItems.AddRange(locations.Where(l => l.ID != exceptId).Select(l => new SelectListItem
            {
                Value = l.ID.ToString(),
                Text = l.Name
            }));

            return locationItems;
        }
    }
}
