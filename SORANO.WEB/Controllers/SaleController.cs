using System;
using System.Collections.Generic;
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
        private readonly IGoodsService _goodsService;
        private readonly IMapper _mapper;

        public SaleController(ISaleService saleService,
            IMapper mapper,
            IUserService userService, 
            IHostingEnvironment environment, 
            IAttachmentTypeService attachmentTypeService, 
            IAttachmentService attachmentService, 
            IMemoryCache memorycache, 
            IGoodsService goodsService) : base(userService, 
                environment, 
                attachmentTypeService, 
                attachmentService, 
                memorycache)
        {
            _saleService = saleService;
            _mapper = mapper;
            _goodsService = goodsService;
        }

        #region GET Actions

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await TryGetActionResultAsync(async () =>
            {
                var showDeleted = Session.GetBool("ShowDeletedSales");

                var salesResult = await _saleService.GetAllAsync(showDeleted, UserId, LocationId);

                if (salesResult.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось получить список продаж.";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ShowDeleted = showDeleted;

                await ClearAttachments();

                var viewModel = _mapper.Map<SaleIndexViewModel>(salesResult.Result);
                viewModel.Mode = SaleTableMode.SaleIndex;
                viewModel.ShowLocation = !LocationId.HasValue;

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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return await TryGetActionResultAsync(async () =>
            {
                var model = new SaleCreateUpdateViewModel { AllowCreation = AllowCreation };

                if (!LocationId.HasValue)
                    return View(model);

                var result = await _saleService.CreateAsync(new SaleDto
                {
                    LocationID = LocationId.Value,
                    UserID = UserId,
                    Date = DateTime.Now
                }, UserId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось оформить продажу.";
                    return RedirectToAction("Index");
                }

                TempData["Success"] = "Продажа оформлена. Для отмены нажмите \"Отменить продажу\". Для подтверждения оформления нажмите \"Подтвердить\".";

                model.ID = result.Result;
                model.LocationID = LocationId.Value;
                model.LocationName = LocationName;
                model.AllowChangeLocation = false;

                model.Groups = await GetGoods(model.LocationID, model.ID, model.ShowSelected);
                model.SelectedCount = model.Groups.Sum(g => g.SelectedCount);
                model.TotalPrice = model.Groups.SelectMany(g => g.Items).Sum(g => Convert.ToDecimal(g.Price)).ToString("0.00") ?? "0.00";

                return View(model);
            }, OnFault);
        }

        private async Task<List<SaleItemsGroupViewModel>> GetGoods(int locationId, int saleId, bool selectedOnly)
        {
            var result = await _goodsService.GetAvailableForLocationAsync(locationId, saleId, selectedOnly);

            var viewModel = new List<SaleItemsGroupViewModel>();

            if (result.Status == ServiceResponseStatus.InvalidOperation)
            {
                TempData["Warning"] = "Необходимо указать магазин для получения списка товаров.";
            }
            else if (result.Status == ServiceResponseStatus.NotFound)
            {
                TempData["Warning"] = "В данном магазине товары отсутствуют.";
            }
            else
            {
                var goods = result.Result.ToList();

                if (selectedOnly)
                    goods = goods.Where(g => g.SaleID.HasValue && g.SaleID == saleId).ToList();

                viewModel = goods.GroupBy(g => new
                {
                    g.DeliveryItem.ArticleID
                }).Select(group =>
                {
                    var items = group.AsEnumerable().ToList();
                    var first = items.First();

                    var model = new SaleItemsGroupViewModel
                    {
                        ArticleName = first.DeliveryItem.Article.Name,
                        ArticleTypeName = first.DeliveryItem.Article.Type.Name,
                        Count = items.Count,
                        IsSelected = items.All(i => i.SaleID == saleId && !i.IsSold),
                        SelectedCount = items.Count(i => i.SaleID == saleId && !i.IsSold),
                        Price = items.All(i => i.Price == first.Price)
                            ? !first.Price.HasValue
                                ? "0.00"
                                : first.Price.Value.ToString("0.00")
                            : "0.00",
                        MainPicturePath = first.DeliveryItem.Article.MainPicture?.FullPath
                            ?? first.DeliveryItem.Article.Type.MainPicture?.FullPath,
                        Items = items.Select(i => new SaleItemViewModel
                        {
                            ArticleId = i.DeliveryItem.ArticleID,
                            ArticleName = i.DeliveryItem.Article.Name,
                            ArticleTypeId = i.DeliveryItem.Article.TypeID,
                            ArticleTypeName = i.DeliveryItem.Article.Type.Name,
                            GoodsId = i.ID,
                            IsSelected = i.SaleID == saleId && !i.IsSold,
                            Price = i.Price.HasValue ? i.Price.Value.ToString("0.00") : "0.00",
                            Quantity = 1,
                            Recommendations = i.Recommendations.Select(r => new SaleItemRecommendationViewModel
                            {
                                Comment = r.Comment,
                                Value = r.Value.HasValue ? r.Value.Value.ToString("0.00") : "0.00"
                            }).Concat(i.DeliveryItem.Recommendations.Select(r => new SaleItemRecommendationViewModel
                            {
                                Comment = r.Comment,
                                Value = r.Value.HasValue ? r.Value.Value.ToString("0.00") : "0.00"
                            })).Concat(i.DeliveryItem.Delivery.Recommendations.Select(r => new SaleItemRecommendationViewModel
                            {
                                Comment = r.Comment,
                                Value = r.Value.HasValue ? r.Value.Value.ToString("0.00") : "0.00"
                            })).ToList()
                        }).ToList()
                    };

                    model.GoodsIds = model.Items.Select(id => id.GoodsId.ToString()).Aggregate((i, j) => i + ',' + j);
                    return model;
                }).ToList();                
            }

            return viewModel;
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

                model.Groups = await GetGoods(model.LocationID, model.ID, model.ShowSelected);
                model.SelectedCount = model.Groups.Sum(g => g.SelectedCount);
                model.TotalPrice = model.Groups.SelectMany(g => g.Items).Sum(g => Convert.ToDecimal(g.Price)).ToString("0.00") ?? "0.00";

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

                return View(_mapper.Map<SaleDeleteViewModel>(result.Result));
            }, OnFault);
        }

        #endregion

        #region POST Actions

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
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public async Task<IActionResult> AddGoods(SaleCreateUpdateViewModel model, int goodsid, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var price = model.Groups.SelectMany(g => g.Items).Single(i => i.GoodsId == goodsid)?.Price;
                var isPriceValid = !string.IsNullOrEmpty(price) && Regex.IsMatch(price, @"^\d+(\.\d{0,2})?$");
                decimal? validPrice = null;
                if (isPriceValid)
                {
                    validPrice = Convert.ToDecimal(price);
                }

                var result = await _saleService.AddGoodsAsync(goodsid, validPrice,  model.ID, UserId);

                if (result.Status == ServiceResponseStatus.NotFound)
                {
                    TempData["Error"] = "Не удалось добавить выбранный товар. Возможно, выбранный товар уже продан.";
                }

                model.Groups = await GetGoods(model.LocationID, model.ID, model.ShowSelected);
                model.SelectedCount = model.Groups.Sum(g => g.SelectedCount);
                model.TotalPrice = model.Groups.SelectMany(g => g.Items).Sum(g => Convert.ToDecimal(g.Price)).ToString("0.00") ?? "0.00";

                return View("Create", model);
            }, ex =>
            {
                TempData["Error"] = ex;
                return View("Create", model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public async Task<IActionResult> AddAllGoods(SaleCreateUpdateViewModel model, string goodsids, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var ids = goodsids.Split(',').Select(id => Convert.ToInt32(id));
                var result = await _saleService.AddGoodsAsync(ids, model.ID, UserId);

                if (result.Status == ServiceResponseStatus.NotFound)
                {
                    TempData["Error"] = "Не удалось добавить выбранные товары. Возможно, некоторые из выбранных товар уже проданы.";
                }

                model.Groups = await GetGoods(model.LocationID, model.ID, model.ShowSelected);
                model.SelectedCount = model.Groups.Sum(g => g.SelectedCount);
                model.TotalPrice = model.Groups.SelectMany(g => g.Items).Sum(g => Convert.ToDecimal(g.Price)).ToString("0.00") ?? "0.00";

                return View("Create", model);
            }, ex =>
            {
                TempData["Error"] = ex;
                return View("Create", model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public async Task<IActionResult> RemoveGoods(SaleCreateUpdateViewModel model, int goodsid, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var result = await _saleService.RemoveGoodsAsync(goodsid, model.ID, UserId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось убрать выбранный товар.";
                }

                model.Groups = await GetGoods(model.LocationID, model.ID, model.ShowSelected);
                model.SelectedCount = model.Groups.Sum(g => g.SelectedCount);
                model.TotalPrice = model.Groups.SelectMany(g => g.Items).Sum(g => Convert.ToDecimal(g.Price)).ToString("0.00") ?? "0.00";

                return View("Create", model);
            }, ex =>
            {
                TempData["Error"] = ex;
                return View("Create", model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public async Task<IActionResult> ShowSelected(SaleCreateUpdateViewModel model, bool show, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                model.ShowSelected = show;
                model.Groups = await GetGoods(model.LocationID, model.ID, model.ShowSelected);
                model.SelectedCount = model.Groups.Sum(g => g.SelectedCount);
                model.TotalPrice = model.Groups.SelectMany(g => g.Items).Sum(g => Convert.ToDecimal(g.Price)).ToString("0.00") ?? "0.00";
                return View("Create", model);
            }, ex =>
            {
                TempData["Error"] = ex;
                return View("Create", model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public async Task<IActionResult> Refresh(SaleCreateUpdateViewModel model, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                model.Groups = await GetGoods(model.LocationID, model.ID, model.ShowSelected);
                model.SelectedCount = model.Groups.Sum(g => g.SelectedCount);
                model.TotalPrice = model.Groups.SelectMany(g => g.Items).Sum(g => Convert.ToDecimal(g.Price)).ToString("0.00") ?? "0.00";
                return View("Create", model);
            }, ex =>
            {
                TempData["Error"] = ex;
                return View("Create", model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LoadAttachments]
        public async Task<IActionResult> RemoveAllGoods(SaleCreateUpdateViewModel model, string goodsids, IFormFile mainPictureFile, IFormFileCollection attachments)
        {
            return await TryGetActionResultAsync(async () =>
            {
                var ids = goodsids.Split(',').Select(id => Convert.ToInt32(id));
                var result = await _saleService.RemoveGoodsAsync(ids, model.ID, UserId);

                if (result.Status != ServiceResponseStatus.Success)
                {
                    TempData["Error"] = "Не удалось убрать выбранные товары.";
                }

                model.Groups = await GetGoods(model.LocationID, model.ID, model.ShowSelected);
                model.SelectedCount = model.Groups.Sum(g => g.SelectedCount);
                model.TotalPrice = model.Groups.SelectMany(g => g.Items).Sum(g => Convert.ToDecimal(g.Price)).ToString("0.00") ?? "0.00";

                return View("Create", model);
            }, ex =>
            {
                TempData["Error"] = ex;
                return View("Create", model);
            });
        }

        #endregion

        private IActionResult OnFault(string ex)
        {
            TempData["Error"] = ex;
            return RedirectToAction("Index");
        }
    }
}