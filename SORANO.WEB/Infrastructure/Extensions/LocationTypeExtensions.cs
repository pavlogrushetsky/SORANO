using SORANO.CORE.StockEntities;
using SORANO.WEB.Models.LocationType;
using System.Linq;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class LocationTypeExtensions
    {
        public static LocationTypeModel ToModel(this LocationType locationType, bool deep = true)
        {
            var model = new LocationTypeModel
            {
                ID = locationType.ID,
                Name = locationType.Name,
                Description = locationType.Description,
                Recommendations = locationType.Recommendations?.Select(r => r.ToModel()).ToList(),
                CanBeDeleted = !locationType.Locations.Any(l => !l.IsDeleted) && !locationType.IsDeleted,
                Created = locationType.CreatedDate.ToString("dd.MM.yyyy"),
                Modified = locationType.ModifiedDate.ToString("dd.MM.yyyy"),
                CreatedBy = locationType.CreatedByUser?.Login,
                ModifiedBy = locationType.ModifiedByUser?.Login
            };

            if (deep)
            {
                model.Locations = locationType.Locations?.Select(l => l.ToModel()).ToList();
            }

            return model;
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
