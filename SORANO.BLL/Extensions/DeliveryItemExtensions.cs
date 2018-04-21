using System.Collections.Generic;
using System.Linq;
using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class DeliveryItemExtensions
    {
        public static DeliveryItemDto ToDto(this DeliveryItem model)
        {
            var dto = new DeliveryItemDto
            {
                ID = model.ID,
                DeliveryID = model.DeliveryID,
                Delivery = model.Delivery.ToDto(),
                ArticleID = model.ArticleID,
                Article = model.Article.ToDto(),
                Quantity = model.Quantity,
                UnitPrice = model.UnitPrice,
                GrossPrice = model.GrossPrice,
                Discount = model.Discount,
                DiscountedPrice = model.DiscountedPrice
            };

            dto.MapDetails(model);
            dto.CanBeDeleted = !(model.Delivery.IsSubmitted || model.IsDeleted);

            return dto;
        }

        public static DeliveryItem ToEntity(this DeliveryItemDto dto)
        {
            var entity = new DeliveryItem
            {
                ID = dto.ID,
                DeliveryID = dto.DeliveryID,
                ArticleID = dto.ArticleID,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                GrossPrice = dto.GrossPrice,
                Discount = dto.Discount,
                DiscountedPrice = dto.DiscountedPrice,
                Recommendations = dto.Recommendations.Select(r => r.ToEntity()).ToList(),
                Attachments = dto.Attachments.Select(a => a.ToEntity()).ToList()
            };

            if (!string.IsNullOrEmpty(dto.MainPicture?.FullPath))
                entity.Attachments.Add(dto.MainPicture.ToEntity());

            return entity;
        }

        public static IEnumerable<DeliveryItemDto> Enumerate(this IReadOnlyList<DeliveryItemDto> items)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                item.Number = i + 1;
                yield return item;
            }
        }

        public static void UpdateFields(this DeliveryItem existentDelivery, DeliveryItem newDelivery)
        {
            existentDelivery.ArticleID = newDelivery.ArticleID;
            existentDelivery.Quantity = newDelivery.Quantity;
            existentDelivery.UnitPrice = newDelivery.UnitPrice;
            existentDelivery.GrossPrice = newDelivery.GrossPrice;
            existentDelivery.Discount = newDelivery.Discount;
            existentDelivery.DiscountedPrice = newDelivery.DiscountedPrice;
        }
    }
}
