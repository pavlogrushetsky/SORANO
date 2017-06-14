using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Models.Recommendation;
using SORANO.WEB.Models.Supplier;
using System.Linq;
using System.Threading.Tasks;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class SupplierController : BaseController
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService, IUserService userService) : base(userService)
        {
            _supplierService = supplierService;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierService.GetAllAsync();

            return View(suppliers.Select(s => s.ToModel()).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new SupplierModel());
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var supplier = await _supplierService.GetAsync(id);

            var model = supplier.ToModel();

            ViewData["IsEdit"] = true;

            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _supplierService.GetAsync(id);

            return View(supplier.ToModel());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _supplierService.GetIncludeAllAsync(id);

            return View(supplier.ToModel());
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierModel model)
        {
            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                // Check the model
                if (!ModelState.IsValid)
                {
                    ModelState.RemoveDuplicateErrorMessages();
                    return View(model);
                }

                // Convert model to supplier entity
                var supplier = model.ToEntity();

                // Get current user
                var currentUser = await GetCurrentUser();

                // Call correspondent service method to create new supplier
                supplier = await _supplierService.CreateAsync(supplier, currentUser.ID);

                // If succeeded
                if (supplier != null)
                {
                    return RedirectToAction("Index", "Supplier");
                }

                // If failed
                ModelState.AddModelError("", "Не удалось создать нового поставщика.");
                return View(model);
            }, (ex) =>
            {
                ModelState.AddModelError("", ex);
                return View(model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(SupplierModel model)
        {
            // Try to get result
            return await TryGetActionResultAsync(async () =>
            {
                // Check the model
                if (!ModelState.IsValid)
                {
                    ViewData["IsEdit"] = true;
                    ModelState.RemoveDuplicateErrorMessages();
                    return View("Create", model);
                }

                // Convert model to supplier entity
                var supplier = model.ToEntity();

                // Get current user
                var currentUser = await GetCurrentUser();

                // Call correspondent service method to update supplier
                supplier = await _supplierService.UpdateAsync(supplier, currentUser.ID);

                // If succeeded
                if (supplier != null)
                {
                    return RedirectToAction("Index", "Supplier");
                }

                // If failed
                ModelState.AddModelError("", "Не удалось обновить поставщика.");
                ViewData["IsEdit"] = true;
                return View("Create", model);
            }, (ex) =>
            {
                ModelState.AddModelError("", ex);
                ViewData["IsEdit"] = true;
                return View("Create", model);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(SupplierModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var currentUser = await GetCurrentUser();

                await _supplierService.DeleteAsync(model.ID, currentUser.ID);

                return RedirectToAction("Index", "Supplier");
            }, (ex) =>
            {
                return RedirectToAction("Index", "Supplier");
            });
        }

        [HttpPost]
        public IActionResult AddRecommendation(SupplierModel supplier, bool isEdit)
        {
            ModelState.Clear();

            supplier.Recommendations.Add(new RecommendationModel());

            ViewData["IsEdit"] = isEdit;

            return View("Create", supplier);
        }

        [HttpPost]
        public IActionResult DeleteRecommendation(SupplierModel supplier, bool isEdit, int num)
        {
            ModelState.Clear();

            supplier.Recommendations.RemoveAt(num);

            ViewData["IsEdit"] = isEdit;

            return View("Create", supplier);
        }

        #endregion
    }
}
