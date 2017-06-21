using SORANO.CORE.StockEntities;
using SORANO.WEB.Models.Location;
using System.Linq;

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
                Recommendations = location.Recommendations?.Select(r => r.ToModel()).ToList(),
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
            return new Location
            {
                ID = model.ID,
                Name = model.Name,
                Comment = model.Comment,
                Recommendations = model.Recommendations.Select(r => r.ToEntity()).ToList(),
                TypeID = int.Parse(model.TypeID)
            };
        }
    }
}
