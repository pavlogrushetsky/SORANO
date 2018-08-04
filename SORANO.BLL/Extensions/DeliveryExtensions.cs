using System.Linq;
using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class DeliveryExtensions
    {
        public static DeliveryDto ToDto(this Delivery model)
        {
            var dto = new DeliveryDto
            {
                ID = model.ID,
                SupplierID = model.SupplierID,
                Supplier = model.Supplier?.ToDto(),
                LocationID = model.LocationID,
                Location = model.DeliveryLocation?.ToDto(),
                BillNumber = model.BillNumber,
                DeliveryDate = model.DeliveryDate,
                PaymentDate = model.PaymentDate,
                DollarRate = model.DollarRate,
                EuroRate = model.EuroRate,
                TotalGrossPrice = model.TotalGrossPrice,
                TotalDiscount = model.TotalDiscount,
                TotalDiscountedPrice = model.TotalDiscountedPrice,
                IsSubmitted = model.IsSubmitted,
                Items = model.Items?.Where(di => !di.IsDeleted).Select(i => i.ToDto())
            };

            dto.MapDetails(model);
            dto.CanBeDeleted = !(model.IsSubmitted || model.IsDeleted);

            return dto;
        }

        public static Delivery ToEntity(this DeliveryDto dto)
        {
            var entity = new Delivery
            {
                ID = dto.ID,
                SupplierID = dto.SupplierID,
                LocationID = dto.LocationID,
                BillNumber = dto.BillNumber,
                DeliveryDate = dto.DeliveryDate,
                PaymentDate = dto.PaymentDate,
                DollarRate = dto.DollarRate,
                EuroRate = dto.EuroRate,
                TotalGrossPrice = dto.TotalGrossPrice,
                TotalDiscount = dto.TotalDiscount,
                TotalDiscountedPrice = dto.TotalDiscountedPrice,
                IsSubmitted = dto.IsSubmitted,
                Items = dto.Items?.Select(i => i.ToEntity()).ToList(),
                Recommendations = dto.Recommendations?.Select(r => r.ToEntity()).ToList(),
                Attachments = dto.Attachments?.Select(a => a.ToEntity()).ToList()
            };

            if (!string.IsNullOrEmpty(dto.MainPicture?.FullPath))
                entity.Attachments.Add(dto.MainPicture.ToEntity());

            return entity;
        }

        public static DeliveryItemsSummaryDto GetSummary(this Delivery delivery)
        {
            return new DeliveryItemsSummaryDto
            {
                TotalDiscount = delivery.TotalDiscount,
                TotalGrossPrice = delivery.TotalGrossPrice,
                TotalDiscountedPrice = delivery.TotalDiscountedPrice
            };
        }

        public static void UpdateFields(this Delivery existentDelivery, Delivery newDelivery)
        {
            existentDelivery.BillNumber = newDelivery.BillNumber;
            existentDelivery.DeliveryDate = newDelivery.DeliveryDate;
            existentDelivery.LocationID = newDelivery.LocationID;
            existentDelivery.DollarRate = newDelivery.DollarRate;
            existentDelivery.EuroRate = newDelivery.EuroRate;
            existentDelivery.IsSubmitted = newDelivery.IsSubmitted;
            existentDelivery.PaymentDate = newDelivery.PaymentDate;
            existentDelivery.SupplierID = newDelivery.SupplierID;
        }
    }
}
