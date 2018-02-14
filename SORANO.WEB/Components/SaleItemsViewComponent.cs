using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.ViewModels.Sale;

namespace SORANO.WEB.Components
{
    public class SaleItemsViewComponent : ViewComponent
    {
        private readonly IGoodsService _goodsService;
        private readonly IMapper _mapper;

        public SaleItemsViewComponent(IGoodsService goodsService, IMapper mapper, ISaleService saleService)
        {
            _goodsService = goodsService;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync(int locationId, int saleId, string selectedCurrency, bool selectedOnly = false)
        {
            var result = await _goodsService.GetAvailableForLocationAsync(locationId, saleId, selectedOnly);

            var viewModel = new List<SaleItemsGroupViewModel>();         

            if (result.Status == ServiceResponseStatus.InvalidOperation)
            {
                TempData["Warning"] = "Необходимо указать магазин для получения списка товаров.";
                ViewBag.SelectedCount = 0;
                ViewBag.TotalPrice = "0.00";
            }
            else if(result.Status == ServiceResponseStatus.NotFound)
            {
                TempData["Warning"] = "В данном магазине товары отсутствуют.";
                ViewBag.SelectedCount = 0;
                ViewBag.TotalPrice = "0.00";
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
                            }).ToList()
                        }).ToList()
                    };

                    model.GoodsIds = model.Items.Select(id => id.GoodsId.ToString()).Aggregate((i, j) => i + ',' + j);
                    return model;
                }).ToList();

                ViewBag.SelectedCount = viewModel.Sum(i => i.SelectedCount);
                ViewBag.TotalPrice = goods.Sum(g => g.Price)?.ToString("0.00") ?? "0.00";                
            }

            ViewBag.SelectedCurrency = selectedCurrency;

            return View(viewModel);
        }
    }
}