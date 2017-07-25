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
                UnitPrice = item.UnitPrice,
                GrossPrice = item.GrossPrice,
                Discount = item.Discount,
                DiscountPrice = item.DiscountedPrice,
                Delivery = item.Delivery?.ToModel(),
                Article = item.Article?.ToModel()
            };
        }
    }
}