using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class DeliveryExtensions
    {
        public static DeliveryDto ToDto(this Delivery model)
        {
            return new DeliveryDto
            {

            };
        }

        public static Delivery ToEntity(this DeliveryDto dto)
        {
            return new Delivery
            {

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
            existentDelivery.TotalDiscount = newDelivery.TotalDiscount;
            existentDelivery.TotalDiscountedPrice = newDelivery.TotalDiscountedPrice;
            existentDelivery.TotalGrossPrice = newDelivery.TotalGrossPrice;
        }
    }
}
