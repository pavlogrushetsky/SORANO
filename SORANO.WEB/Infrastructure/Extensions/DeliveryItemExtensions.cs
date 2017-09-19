using SORANO.CORE.StockEntities;
using System.Globalization;
using SORANO.WEB.ViewModels;

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
                ArticleID = item.ArticleID.ToString(),
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice.ToString("0.00"),
                GrossPrice = item.GrossPrice.ToString("0.00"),
                Discount = item.Discount.ToString("0.00"),
                DiscountPrice = item.DiscountedPrice.ToString("0.00"),
                //Article = item.Article?.ToModel()
            };
        }

        public static DeliveryItem ToEntity(this DeliveryItemModel model)
        {
            var deliveryItem = new DeliveryItem
            {
                ID = model.ID,
                UnitPrice = decimal.Parse(model.UnitPrice, NumberStyles.Any, new CultureInfo("en-US")),
                GrossPrice = decimal.Parse(model.GrossPrice, NumberStyles.Any, new CultureInfo("en-US")),
                Discount = decimal.Parse(model.Discount, NumberStyles.Any, new CultureInfo("en-US")),
                DiscountedPrice = decimal.Parse(model.DiscountPrice, NumberStyles.Any, new CultureInfo("en-US")),
                ArticleID = int.Parse(model.ArticleID),
                Quantity = model.Quantity
            };

            return deliveryItem;
        }
    }
}