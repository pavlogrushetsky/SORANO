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

        public async Task<IViewComponentResult> InvokeAsync(int saleId, int locationId, bool selectedOnly)
        {
            var result = await _goodsService.GetAvailableForLocationAsync(locationId, saleId, selectedOnly);

            var viewModel = new SaleItemsGroupsViewModel
            {
                Summary = new SaleItemsSummaryViewModel()
            };         

            if (result.Status == ServiceResponseStatus.InvalidOperation)
            {
                viewModel.Warning = "Необходимо указать магазин для получения списка товаров.";
                viewModel.Summary.SelectedCount = 0;
                viewModel.Summary.TotalPrice = "0.00";
            }
            else if(result.Status == ServiceResponseStatus.NotFound)
            {
                viewModel.Warning = "В данном магазине товары отсутствуют.";
                viewModel.Summary.SelectedCount = 0;
                viewModel.Summary.TotalPrice = "0.00";
            }
            else
            {
                var goods = result.Result.ToList();

                if (selectedOnly)
                    goods = goods.Where(g => g.SaleID.HasValue && g.SaleID == saleId).ToList();

                viewModel.Groups = goods.GroupBy(g => new
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

                viewModel.Summary.SelectedCount = viewModel.Groups.Sum(i => i.SelectedCount);
                viewModel.Summary.TotalPrice = goods.Sum(g => g.Price)?.ToString("0.00") ?? "0.00";                
            }

            //viewModel.Summary.SelectedCurrency = selectedCurrency;

            return View(viewModel);
        }
    }
}