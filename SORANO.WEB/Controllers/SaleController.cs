using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Dtos;
using SORANO.BLL.Services;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Extensions;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels.Sale;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager,editor,user")]
    [CheckUser]
    public class SaleController : EntityBaseController<SaleCreateUpdateViewModel>
    {
        private readonly ISaleService _saleService;
        private readonly IMapper _mapper;

        public SaleController(ISaleService saleService,
            IMapper mapper,
            IUserService userService, 
            IExceptionService exceptionService,
            IHostingEnvironment environment, 
            IAttachmentTypeService attachmentTypeService, 
            IAttachmentService attachmentService, 
            IMemoryCache memorycache) : base(userService, 
                exceptionService,
                environment, 
                attachmentTypeService, 
                attachmentService, 
                memorycache)
        {
            _saleService = saleService;
            _mapper = mapper;
        }

        #region GET Actions

        [HttpGet]
        public IActionResult Index()
        {
            return TryGetActionResult(() =>
            {
                var showDeleted = Session.GetBool("ShowDeletedSales");

                var salesResult = _saleService.GetAll(showDeleted, LocationId);

                if (salesResult.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить список продаж.";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ShowDeleted = showDeleted;

                ClearAttachments();

                var viewModel = _mapper.Map<SaleIndexViewModel>(salesResult.Result);
                viewModel.Mode = SaleTableMode.SaleIndex;
                viewModel.ShowLocation = !LocationId.HasValue;
                foreach (var item in viewModel.Items)
                {
                    item.CanBeUpdated = IsEditor || !item.IsSubmitted && !item.IsDeleted;
                    item.CanBeDeleted = IsEditor || item.CanBeDeleted;
                }

                if (!LocationId.HasValue)
                {
                    TempData["Warning"] = "Для создания продажи необходимо войти в систему, указав место.";
                }

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
            Session.SetBool("ShowDeletedSales", show);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _saleService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанную продажу.";
                    return RedirectToAction("Index");
                }

                var viewModel = _mapper.Map<SaleDetailsViewModel>(result.Result);
                viewModel.Items = result.Result.Goods.GroupBy(g => new
                    {
                        g.DeliveryItem.Article.ID,
                        g.Price
                    }).Select(group =>
                    {
                        var first = group.First();

                        var model = new SaleItemDetailsViewModel
                        {
                            ArticleId = first.DeliveryItem.Article.ID,
                            ArticleName = first.DeliveryItem.Article.Name,
                            Price = group.Key.Price?.ToString("0.00", new CultureInfo("ru-RU")) ?? "0,00",
                            Quantity = group.Count()
                        };

                        return model;
                    })
                    .ToList();

                return View(viewModel);

            }, OnFault);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string ids, int? locationId, string locationName)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var model = new SaleCreateUpdateViewModel { AllowCreation = AllowCreation };

                if (!LocationId.HasValue && !locationId.HasValue)
                {
                    model.AllowChangeLocation = true;
                    return View(model);
                }                    

                var result = await _saleService.CreateAsync(new SaleDto
                {
                    LocationID = locationId ?? LocationId.Value,
                    UserID = UserId,
                    Date = DateTime.Now
                }, UserId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось оформить продажу.";
                    return RedirectToAction("Index");
                }

                if (!string.IsNullOrWhiteSpace(ids))
                {
                    var goodsIds = ids.Split(',').Select(id => Convert.ToInt32(id));
                    var goodsResult = await _saleService.AddGoodsAsync(goodsIds, null, result.Result, UserId);
                    if (goodsResult.Status != ServiceResponseStatus.Success)
                    {
                        await _saleService.DeleteAsync(result.Result, UserId);

                        TempData["Error"] = "Не удалось оформить продажу.";
                        return RedirectToAction("Index");
                    }
                }                

                TempData["Success"] = "Продажа оформлена. Для отмены нажмите \"Отменить продажу\". Для подтверждения оформления нажмите \"Подтвердить\".";

                model.ID = result.Result;
                model.LocationID = locationId ?? LocationId.Value;
                model.LocationName = locationName ?? LocationName;
                model.AllowChangeLocation = false;                

                return View(model);
            }, OnFault);
        }
        
        public async Task<IActionResult> Update(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _saleService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанную поставку.";
                    return RedirectToAction("Index");
                }

                var model = _mapper.Map<SaleCreateUpdateViewModel>(result.Result);
                model.IsUpdate = true;
                model.AllowChangeLocation = false;
                model.ShowSelected = true;

                return View("Create", model);
            }, OnFault);
        }

        public async Task<IActionResult> Delete(int id)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _saleService.GetAsync(id);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось найти указанную продажу.";
                    return RedirectToAction("Index");
                }

                var model = _mapper.Map<SaleDeleteViewModel>(result.Result);
                model.CanBeDeleted = IsEditor || model.CanBeDeleted;

                return View(model);
            }, OnFault);
        }

        #endregion

        #region POST Actions

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public async Task<IActionResult> Create(SaleCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor(nameof(model.IsSubmitted));

                if (model.IsSubmitted)
                {
                    var itemsValid = await _saleService.ValidateItemsForAsync(model.ID, model.IsWriteOff);
                    if (itemsValid.Status != ServiceResponseStatus.Success || !itemsValid.Result)
                    {
                        ModelState.AddModelError("", "Продажа должна содержать товары.");

                        if (!model.IsWriteOff)
                            ModelState.AddModelError("", "Для всех товаров должна быть установлена цена.");
                    }
                }

                if (!ModelState.IsValid)
                {
                    model.IsSubmitted = false;
                    return View(model);
                }

                var sale = _mapper.Map<SaleDto>(model);

                var result = await _saleService.UpdateAsync(sale, UserId);
                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось обновить продажу.";
                    return RedirectToAction("Index");
                }

                TempData["Success"] = "Продажа была успешно обновлена.";
                return RedirectToAction("Index");

            }, OnFault);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public async Task<IActionResult> Update(SaleCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                ModelState.RemoveFor(nameof(model.IsSubmitted));

                if (model.IsSubmitted)
                {
                    var itemsValid = await _saleService.ValidateItemsForAsync(model.ID, model.IsWriteOff);
                    if (itemsValid.Status != ServiceResponseStatus.Success || !itemsValid.Result)
                    {
                        ModelState.AddModelError("", "Продажа должна содержать товары.");

                        if (!model.IsWriteOff)
                            ModelState.AddModelError("", "Для всех товаров должна быть установлена цена.");
                    }
                }

                if (!ModelState.IsValid)
                {
                    model.IsSubmitted = false;
                    return View("Create", model);
                }

                var sale = _mapper.Map<SaleDto>(model);

                var result = await _saleService.UpdateAsync(sale, UserId);
                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось обновить продажу.";
                    return RedirectToAction("Index");
                }

                TempData["Success"] = "Продажа была успешно обновлена.";
                return RedirectToAction("Index");

            }, OnFault);
        }

        [HttpPost]
        public IActionResult Refresh(int saleId, int locationId, bool selectedOnly, string searchCriteria)
        {
            return ViewComponent("SaleItems", new { saleId, locationId, selectedOnly, searchCriteria });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(SaleDeleteViewModel model)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _saleService.DeleteAsync(model.ID, UserId);

                if (result.Status == ServiceResponseStatus.Success)
                {
                    TempData["Success"] = "Оформление продажи было успешно отменено.";
                }
                else
                {
                    TempData["Error"] = "Не удалось отменить оформление продажи.";
                }

                return RedirectToAction("Index");
            }, OnFault);
        }

        [HttpPost]
        public async Task<JsonResult> AddGoods(int saleId, int goodsId, string price)
        {
            try
            {
                var isPriceValid = !string.IsNullOrEmpty(price) && Regex.IsMatch(price, @"^\d+(\,\d{0,2})?$");
                decimal validPrice;
                if (isPriceValid)
                {
                    decimal.TryParse(price, NumberStyles.Any, new CultureInfo("ru-RU"), out validPrice);
                }
                else
                {
                    return Json(null);
                }

                var summary = await _saleService.AddGoodsAsync(goodsId, validPrice, saleId, UserId);
                var model = _mapper.Map<SaleItemsSummaryViewModel>(summary.Result);
                return Json(model);
            }
            catch (Exception)
            {
                return Json(null);
            }
        }

        [HttpPost]
        public async Task<JsonResult> RemoveGoods(int saleId, int goodsId)
        {
            try
            {
                var summary = await _saleService.RemoveGoodsAsync(goodsId, saleId, UserId);
                var model = _mapper.Map<SaleItemsSummaryViewModel>(summary.Result);
                return Json(model);
            }
            catch (Exception)
            {
                return Json(null);
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddAllGoods(int saleId, string goodsIds, string price)
        {
            try
            {
                var isPriceValid = !string.IsNullOrEmpty(price) && Regex.IsMatch(price, @"^\d+(\,\d{0,2})?$");
                decimal validPrice;
                if (isPriceValid)
                {
                    decimal.TryParse(price, NumberStyles.Any, new CultureInfo("ru-RU"), out validPrice);
                }
                else
                {
                    return Json(false);
                }

                var ids = goodsIds.Split(',').Select(id => Convert.ToInt32(id));
                var summary = await _saleService.AddGoodsAsync(ids, validPrice, saleId, UserId);
                var model = _mapper.Map<SaleItemsSummaryViewModel>(summary.Result);
                return Json(model);
            }
            catch (Exception)
            {
                return Json(null);
            }
        }

        [HttpPost]
        public async Task<JsonResult> RemoveAllGoods(int saleId, string goodsIds)
        {
            try
            {
                var ids = goodsIds.Split(',').Select(id => Convert.ToInt32(id));
                var summary = await _saleService.RemoveGoodsAsync(ids, saleId, UserId);
                var model = _mapper.Map<SaleItemsSummaryViewModel>(summary.Result);
                return Json(model);
            }
            catch (Exception)
            {
                return Json(null);
            }
        }

        #endregion

        [HttpPost]
        public JsonResult GetSales(int locationId)
        {
            var sales = _saleService.GetAll(false, locationId);

            return Json(new
            {
                results = sales.Result?
                    .Where(s => !s.IsSubmitted && s.Date.HasValue)
                    .OrderByDescending(s => s.Date)
                    .Select(s => new
                    {
                        id = s.ID,
                        text = s.Client == null 
                            ? s.Date.Value.ToString("dd.MM.yyyy") 
                            : $"{s.Date.Value:dd.MM.yyyy}: {s.Client.Name}"
                    })
            });
        }

        private IActionResult OnFault(string ex)
        {
            TempData["Error"] = ex;
            return RedirectToAction("Index");
        }
    }
}