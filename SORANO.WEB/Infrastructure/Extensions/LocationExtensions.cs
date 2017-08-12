using SORANO.CORE.StockEntities;
using System.Linq;
using SORANO.WEB.Models;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class LocationExtensions
    {
        public static LocationModel ToModel(this Location location)
        {
            return new LocationModel
            {
                ID = location.ID,
                Name = location.Name,
                Comment = location.Comment,
                TypeID = location.TypeID.ToString(),
                Type = location.Type?.ToModel(false),
                Goods = location.Storages.Select(s => s.Goods).GroupBy(g => g.DeliveryItemID).Select(gr => 
                {
                    var delivery = gr.First().DeliveryItem.Delivery;
                    var symbol = delivery.EuroRate.HasValue ? "€" : delivery.DollarRate.HasValue ? "$" : "₴";

                    return new StoredGoodsModel
                    {
                        Article = gr.First().DeliveryItem.Article.Name,
                        ArticleID = gr.First().DeliveryItem.Article.ID,
                        Quantity = gr.Count(),
                        DeliveredPrice = gr.First().DeliveryItem.UnitPrice.ToString("0.00") + " " + symbol,
                        DeliveryID = delivery.ID,
                        DeliveryItemID = gr.First().DeliveryItem.ID,
                        BillNumber = delivery.BillNumber
                    };
                }).ToList(),
                Recommendations = location.Recommendations?.Where(r => !r.IsDeleted).Select(r => r.ToModel()).ToList(),
                MainPicture = location.Attachments?.SingleOrDefault(a => !a.IsDeleted && a.Type.Name.Equals("Основное изображение"))?.ToModel() ?? new AttachmentModel(),
                Attachments = location.Attachments?.Where(a => !a.IsDeleted && !a.Type.Name.Equals("Основное изображение")).Select(a => a.ToModel()).ToList(),
                CanBeDeleted = !location.Storages.Any() || !location.SoldGoods.Any(),
                IsDeleted = location.IsDeleted,
                Created = location.CreatedDate.ToString("dd.MM.yyyy"),
                Modified = location.ModifiedDate.ToString("dd.MM.yyyy"),
                Deleted = location.DeletedDate?.ToString("dd.MM.yyyy"),
                CreatedBy = location.CreatedByUser?.Login,
                ModifiedBy = location.ModifiedByUser?.Login,
                DeletedBy = location.DeletedByUser?.Login
            };
        }

        public static Location ToEntity(this LocationModel model)
        {
            var location = new Location
            {
                ID = model.ID,
                Name = model.Name,
                Comment = model.Comment,
                Recommendations = model.Recommendations.Select(r => r.ToEntity()).ToList(),
                TypeID = int.Parse(model.TypeID),
                Attachments = model.Attachments.Select(a => a.ToEntity()).ToList()
            };

            if (!string.IsNullOrEmpty(model.MainPicture?.Name))
            {
                location.Attachments.Add(model.MainPicture.ToEntity());
            }

            return location;
        }
    }
}
