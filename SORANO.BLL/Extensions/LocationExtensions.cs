using System.Linq;
using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class LocationExtensions
    {
        public static LocationDto ToDto(this Location model)
        {
            var dto = new LocationDto
            {
                ID = model.ID,
                Name = model.Name,
                Comment = model.Comment,
                TypeID = model.TypeID,
                Type = model.Type.ToDto()
            };

            dto.MapDetails(model);
            dto.CanBeDeleted = !model.Deliveries.Any() && !model.Storages.Any() && !model.SoldGoods.Any() && !model.IsDeleted;

            return dto;
        }

        public static Location ToEntity(this LocationDto dto)
        {
            var entity = new Location
            {
                ID = dto.ID,
                Name = dto.Name,
                Comment = dto.Comment,
                TypeID = dto.TypeID,
                Recommendations = dto.Recommendations.Select(r => r.ToEntity()).ToList(),
                Attachments = dto.Attachments.Select(a => a.ToEntity()).ToList()
            };

            if (!string.IsNullOrEmpty(dto.MainPicture?.FullPath))
                entity.Attachments.Add(dto.MainPicture.ToEntity());

            return entity;
        }

        public static void UpdateFields(this Location existentLocation, Location newLocation)
        {
            existentLocation.Name = newLocation.Name;
            existentLocation.Comment = newLocation.Comment;
            existentLocation.TypeID = newLocation.TypeID;
        }
    }
}
