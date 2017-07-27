using SORANO.CORE.StockEntities;
using SORANO.WEB.Models;
using System.Linq;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class DeliveryExtensions
    {
        public static DeliveryModel ToModel(this Delivery delivery)
        {
            return new DeliveryModel
            {
                ID = delivery.ID,
                BillNumber = delivery.BillNumber,
                DeliveryDate = delivery.DeliveryDate,
                PaymentDate = delivery.PaymentDate,
                DollarRate = delivery.DollarRate,
                EuroRate = delivery.EuroRate,
                TotalGrossPrice = delivery.TotalGrossPrice,
                TotalDiscount = delivery.TotalDiscount,
                TotalDiscountPrice = delivery.TotalDiscountedPrice,
                Status = delivery.IsSubmitted,
                Supplier = delivery.Supplier?.ToModel(),
                SupplierID = delivery.SupplierID.ToString(),
                Location = delivery.DeliveryLocation.ToModel(),
                LocationID = delivery.DeliveryLocation.ID.ToString(),
                DeliveryItems = delivery.Items.Select(i => i.ToModel()).ToList()
            };
        }

        public static Delivery ToEntity(this DeliveryModel model)
        {
            var delivery = new Delivery
            {
                ID = model.ID,
                BillNumber = model.BillNumber,
                DeliveryDate = model.DeliveryDate,
                PaymentDate = model.PaymentDate,
                DollarRate = model.DollarRate,
                EuroRate = model.EuroRate,
                TotalGrossPrice = model.TotalGrossPrice,
                TotalDiscount = model.TotalDiscount,
                TotalDiscountedPrice = model.TotalDiscountPrice,
                SupplierID = int.Parse(model.SupplierID),
                LocationID = int.Parse(model.LocationID),
                Recommendations = model.Recommendations.Select(r => r.ToEntity()).ToList(),
                Attachments = model.Attachments.Select(a => a.ToEntity()).ToList(),
                Items = model.DeliveryItems.Select(di => di.ToEntity()).ToList()
            };

            if (!string.IsNullOrEmpty(model.MainPicture?.Name))
            {
                delivery.Attachments.Add(model.MainPicture.ToEntity());
            }

            return delivery;
        }
    }
}