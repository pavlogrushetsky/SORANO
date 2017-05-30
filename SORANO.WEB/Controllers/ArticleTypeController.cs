using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Models.ArticleType;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class ArticleTypeController : Controller
    {
        private readonly IArticleTypeService _articleTypeService;
        private readonly IUserService _userService;

        public ArticleTypeController(IArticleTypeService articleTypeService, IUserService userService)
        {
            _articleTypeService = articleTypeService;
            _userService = userService;
        }

        #region GET Actions

        [HttpGet]
        public IActionResult Create()
        {
            var model = TempData.Get<ArticleTypeModel>("ArticleTypeModel") ?? new ArticleTypeModel();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var type = await _articleTypeService.GetAsync(id);

            return PartialView("_Details", type.ToModel());
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var articleType = await _articleTypeService.GetAsync(id);

            var model = TempData.Get<ArticleTypeModel>("ArticleTypeModel") ?? articleType.ToModel();

            ViewData["IsEdit"] = true;

            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Select(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                return BadRequest();
            }

            var typeModel = TempData.Get<ArticleTypeModel>("ArticleTypeModel");

            if (typeModel == null)
            {
                return Redirect(returnUrl);
            }

            var types = await _articleTypeService.GetAllAsync();

            var model = new ArticleTypeSelectModel
            {
                Types = types.Select(t => t.ToModel()).ToList(),
                ArticleType = typeModel,
                ReturnUrl = returnUrl
            };

            model.Types.Where(t => t.ID == typeModel.ParentType?.ID).ToList().ForEach(t =>
            {
                t.IsSelected = true;
            });

            return View(model);
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var articleType = new ArticleType();

            articleType.FromCreateModel(model);

            var currentUser = await _userService.GetAsync(HttpContext.User.FindFirst(ClaimTypes.Name)?.Value);

            articleType.CreatedBy = currentUser.ID;
            articleType.ModifiedBy = currentUser.ID;
            articleType.CreatedDate = DateTime.Now;
            articleType.ModifiedDate = DateTime.Now;

            articleType = await _articleTypeService.CreateAsync(articleType);

            if (articleType != null)
            {
                return RedirectToAction("Index", "Article");
            }

            ModelState.AddModelError("", "Не удалось создать новый тип артикулов.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectParentType(ArticleTypeModel model, string returnUrl)
        {
            TempData.Put("ArticleTypeModel", model);

            return RedirectToAction("Select", "ArticleType", new { returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Select(ArticleTypeSelectModel model)
        {
            var typeModel = model.ArticleType;

            var selectedType = model.Types.SingleOrDefault(t => t.IsSelected);

            typeModel.ParentType = selectedType;

            TempData.Put("ArticleTypeModel", typeModel);

            return Redirect(model.ReturnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(ArticleTypeSelectModel model)
        {
            TempData.Put("ArticleTypeModel", model.ArticleType);

            return Redirect(model.ReturnUrl);
        }

        #endregion        
    }
}