using SORANO.CORE.StockEntities;
using SORANO.WEB.Models;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class DeliveryItemExtensions
    {
        public static DeliveryItemModel ToModel(this DeliveryItem item)
        {
            return new DeliveryItemModel
            {
                ID = item.ID,
                DeliveryID = item.DeliveryID,
                ArticleID = item.ArticleID,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice.ToString("0.##"),
                GrossPrice = item.GrossPrice.ToString("0.##"),
                Discount = item.Discount.ToString("0.##"),
                DiscountPrice = item.DiscountedPrice.ToString("0.##"),
                Delivery = item.Delivery?.ToModel(),
                Article = item.Article?.ToModel()
            };
        }
    }
}