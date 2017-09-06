using System.Globalization;
using System.Linq;
using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;
using SORANO.WEB.ViewModels;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class GoodsExtensions
    {
        public static UserSaleModel ToUserSaleModel(this Goods goods)
        {
            return new UserSaleModel
            {
                ArticleName = goods.DeliveryItem.Article.Name,
                Location = goods.SaleLocation.Name,
                Price = goods.SalePrice?.ToString("C", new CultureInfo("uk-UA")),
                Date = goods.SaleDate?.ToString("dd.MM.yyyy")
            };
        }

        public static GoodsIndexModel ToIndexModel(this AllGoodsDTO goodsDto)
        {
            return new GoodsIndexModel
            {
                ArticleId = goodsDto.ArticleId,
                ArticleImage = goodsDto.ArticleImage,
                ArticleName = goodsDto.ArticleName,
                Goods = goodsDto.Goods.Select(g => g.ToGroupModel()).ToList()
            };
        }

        public static GoodsGroupModel ToGroupModel(this GoodsGroupDTO groupDto)
        {
            var price = groupDto.DeliveryPrice.ToString("0.00");

            return new GoodsGroupModel
            {
                BillNumber = groupDto.BillNumber,
                Count = groupDto.Count,
                DeliveryId = groupDto.DeliveryId,
                LocationId = groupDto.LocationId,
                LocationName = groupDto.LocationName,
                DeliveryPrice = groupDto.DollarRate.HasValue ? price + " $" : groupDto.EuroRate.HasValue ? price + " €" : price + " ₴"
            };
        }
    }
}