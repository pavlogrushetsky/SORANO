using SORANO.CORE.StockEntities;
using SORANO.WEB.Models.LocationType;
using System.Linq;
using SORANO.WEB.Models.Attachment;

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
                Recommendations = locationType.Recommendations?.Where(r => !r.IsDeleted).Select(r => r.ToModel()).ToList(),
                MainPicture = locationType.Attachments?.SingleOrDefault(a => !a.IsDeleted && a.Type.Name.Equals("Основное изображение"))?.ToModel() ?? new AttachmentModel(),
                Attachments = locationType.Attachments?.Where(a => !a.IsDeleted && !a.Type.Name.Equals("Основное изображение")).Select(a => a.ToModel()).ToList(),
                CanBeDeleted = locationType.Locations.All(l => l.IsDeleted) && !locationType.IsDeleted,
                IsDeleted = locationType.IsDeleted,
                Created = locationType.CreatedDate.ToString("dd.MM.yyyy"),
                Modified = locationType.ModifiedDate.ToString("dd.MM.yyyy"),
                Deleted = locationType.DeletedDate?.ToString("dd.MM.yyyy"),
                CreatedBy = locationType.CreatedByUser?.Login,
                ModifiedBy = locationType.ModifiedByUser?.Login,
                DeletedBy = locationType.DeletedByUser?.Login
            };

            if (deep)
            {
                model.Locations = locationType.Locations?.Select(l => l.ToModel()).ToList();
            }

            return model;
        }

        public static LocationType ToEntity(this LocationTypeModel model)
        {
            var locationType = new LocationType
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                Recommendations = model.Recommendations.Select(r => r.ToEntity()).ToList(),
                Attachments = model.Attachments.Select(a => a.ToEntity()).ToList()
            };

            if (!string.IsNullOrEmpty(model.MainPicture?.Name))
            {
                locationType.Attachments.Add(model.MainPicture.ToEntity());
            }

            return locationType;
        }
    }
}
