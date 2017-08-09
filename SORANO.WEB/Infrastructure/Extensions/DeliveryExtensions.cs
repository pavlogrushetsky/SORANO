using System;
using System.Globalization;
using SORANO.CORE.StockEntities;
using SORANO.WEB.Models;
using System.Linq;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class DeliveryExtensions
    {
        public static DeliveryModel ToModel(this Delivery delivery)
        {
            var model = new DeliveryModel
            {
                ID = delivery.ID,
                BillNumber = delivery.BillNumber,
                DeliveryDate = delivery.DeliveryDate.ToString("dd.MM.yyyy"),
                PaymentDate = delivery.PaymentDate?.ToString("dd.MM.yyyy"),
                DollarRate = delivery.DollarRate?.ToString("0.00"),
                EuroRate = delivery.EuroRate?.ToString("0.00"),
                TotalGrossPrice = delivery.TotalGrossPrice,
                TotalDiscount = delivery.TotalDiscount,
                TotalDiscountPrice = delivery.TotalDiscountedPrice,
                Status = delivery.IsSubmitted,
                Supplier = delivery.Supplier?.ToModel(),
                SupplierID = delivery.SupplierID.ToString(),
                Location = delivery.DeliveryLocation.ToModel(),
                LocationID = delivery.LocationID.ToString(),
                SelectedCurrency = delivery.EuroRate.HasValue ? 2 : delivery.DollarRate.HasValue ? 1 : 0,
                CanBeDeleted = !delivery.IsSubmitted && !delivery.IsDeleted,
                CanBeUpdated = !delivery.IsSubmitted && !delivery.IsDeleted,
                DeliveryItems = delivery.Items.Select(i => i.ToModel()).ToList(),
                Recommendations = delivery.Recommendations?.Where(r => !r.IsDeleted).Select(r => r.ToModel()).ToList(),
                MainPicture = delivery.Attachments?.SingleOrDefault(a => !a.IsDeleted && a.Type.Name.Equals("Основное изображение"))?.ToModel() ?? new AttachmentModel(),
                Attachments = delivery.Attachments?.Where(a => !a.IsDeleted && !a.Type.Name.Equals("Основное изображение")).Select(a => a.ToModel()).ToList()
            };

            return model;
        }

        public static Delivery ToEntity(this DeliveryModel model)
        {
            var delivery = new Delivery
            {
                ID = model.ID,
                BillNumber = model.BillNumber,
                DeliveryDate = DateTime.ParseExact(model.DeliveryDate, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                PaymentDate = string.IsNullOrEmpty(model.PaymentDate) ? null : (DateTime?)DateTime.ParseExact(model.PaymentDate, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                DollarRate = !string.IsNullOrEmpty(model.DollarRate) ? decimal.Parse(model.DollarRate, NumberStyles.Any, new CultureInfo("en-US")) : (decimal?)null,
                EuroRate = !string.IsNullOrEmpty(model.DollarRate) ? decimal.Parse(model.EuroRate, NumberStyles.Any, new CultureInfo("en-US")) : (decimal?)null,
                TotalGrossPrice = model.TotalGrossPrice,
                TotalDiscount = model.TotalDiscount,
                TotalDiscountedPrice = model.TotalDiscountPrice,
                IsSubmitted = model.Status,
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