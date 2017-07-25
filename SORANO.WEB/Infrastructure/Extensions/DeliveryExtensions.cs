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
                DeliveryDate = delivery.DeliveryDate.ToString("dd.MM.yyyy"),
                PaymentDate = delivery.PaymentDate?.ToString("dd.MM.yyyy"),
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
    }
}