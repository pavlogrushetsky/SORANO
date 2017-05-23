using System.Globalization;
using SORANO.CORE.StockEntities;
using SORANO.WEB.Models.User;

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
    }
}