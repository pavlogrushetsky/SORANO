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
                Article = item.Article?.ToModel()
            };
        }

        public static DeliveryItem ToEntity(this DeliveryItemModel model)
        {
            var deliveryItem = new DeliveryItem
            {
                ID = model.ID,
                UnitPrice = model.UnitPrice,
                GrossPrice = model.GrossPrice,
                Discount = model.Discount,
                DiscountedPrice = model.DiscountPrice,
                ArticleID = model.ArticleID,
                Quantity = model.Quantity
            };

            return deliveryItem;
        }
    }
}