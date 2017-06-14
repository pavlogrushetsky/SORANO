using SORANO.CORE.StockEntities;
using SORANO.WEB.Models.LocationType;
using System.Linq;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class LocationTypeExtensions
    {
        public static LocationTypeModel ToModel(this LocationType locationType)
        {
            return new LocationTypeModel
            {
                ID = locationType.ID,
                Name = locationType.Name,
                Description = locationType.Description,
                Recommendations = locationType.Recommendations?.Select(r => r.ToModel()).ToList(),
                CanBeDeleted = !locationType.Locations.Any(),
                Created = locationType.CreatedDate.ToString("dd.MM.yyyy"),
                Modified = locationType.ModifiedDate.ToString("dd.MM.yyyy"),
                CreatedBy = locationType.CreatedByUser?.Login,
                ModifiedBy = locationType.ModifiedByUser?.Login
            };
        }

        public static LocationType ToEntity(this LocationTypeModel model)
        {
            return new LocationType
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                Recommendations = model.Recommendations.Select(r => r.ToEntity()).ToList()
            };
        }
    }
}
