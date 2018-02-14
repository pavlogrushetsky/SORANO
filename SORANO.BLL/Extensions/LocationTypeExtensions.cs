using System.Linq;
using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class LocationTypeExtensions
    {
        public static LocationTypeDto ToDto(this LocationType model)
        {
            var dto = new LocationTypeDto
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                Locations = model.Locations.Select(l => l.ToDto())
            };

            dto.MapDetails(model);
            dto.CanBeDeleted = model.Locations.All(a => a.IsDeleted) && !model.IsDeleted;

            return dto;
        }

        public static LocationType ToEntity(this LocationTypeDto dto)
        {
            var entity = new LocationType
            {
                ID = dto.ID,
                Name = dto.Name,
                Description = dto.Description,
                Recommendations = dto.Recommendations.Select(r => r.ToEntity()).ToList(),
                Attachments = dto.Attachments.Select(a => a.ToEntity()).ToList()
            };

            if (!string.IsNullOrEmpty(dto.MainPicture?.FullPath))
                entity.Attachments.Add(dto.MainPicture.ToEntity());

            return entity;
        }

        public static void UpdateFields(this LocationType existentLocationType, LocationType newLocationType)
        {
            existentLocationType.Name = newLocationType.Name;
            existentLocationType.Description = newLocationType.Description;
        }
    }
}