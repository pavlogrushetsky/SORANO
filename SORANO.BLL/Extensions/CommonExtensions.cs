using System;
using System.Collections.Generic;
using System.Linq;
using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class CommonExtensions
    {
        public static void MapDetails(this BaseDto dto, StockEntity model)
        {
            dto.IsDeleted = model.IsDeleted;
            dto.CanBeDeleted = !model.IsDeleted;
            dto.Created = model.CreatedDate;
            dto.Modified = model.ModifiedDate;
            dto.Deleted = model.DeletedDate;
            dto.CreatedBy = model.CreatedByUser?.Login;
            dto.ModifiedBy = model.ModifiedByUser?.Login;
            dto.DeletedBy = model.DeletedByUser?.Login;
            dto.Recommendations = model.Recommendations
                .Where(r => !r.IsDeleted)
                .Select(r => r.ToDto());
            dto.Attachments = model.Attachments?
                .Where(a => !a.IsDeleted && !a.Type.Name.Equals("Основное изображение"))
                .Select(a => a.ToDto());
            dto.MainPicture = model.Attachments?
                .Where(a => !a.IsDeleted && a.Type.Name.Equals("Основное изображение"))
                .SingleOrDefault()?
                .ToDto() ?? new AttachmentDto();
        }

        public static StockEntity UpdateCreatedFields(this StockEntity entity, int userId)
        {
            entity.CreatedBy = userId;
            entity.CreatedDate = DateTime.Now;

            return entity;
        }

        public static StockEntity UpdateModifiedFields(this StockEntity entity, int userId)
        {
            entity.ModifiedBy = userId;
            entity.ModifiedDate = DateTime.Now;

            return entity;
        }

        public static StockEntity UpdateDeletedFields(this StockEntity entity, int userId)
        {
            entity.DeletedBy = userId;
            entity.DeletedDate = DateTime.Now;
            entity.IsDeleted = true;

            return entity;
        }

        public static ICollection<Recommendation> UpdateCreatedFields(this ICollection<Recommendation> recommendations, int userId)
        {
            foreach (var recommendation in recommendations)
                recommendation.UpdateCreatedFields(userId);

            return recommendations;
        }

        public static ICollection<Recommendation> UpdateModifiedFields(this ICollection<Recommendation> recommendations, int userId)
        {
            foreach (var recommendation in recommendations)
                recommendation.UpdateModifiedFields(userId);

            return recommendations;
        }

        public static ICollection<Attachment> UpdateCreatedFields(this ICollection<Attachment> attachments, int userId)
        {
            foreach (var attachment in attachments)
                attachment.UpdateCreatedFields(userId);

            return attachments;
        }

        public static ICollection<Attachment> UpdateModifiedFields(this ICollection<Attachment> attachments, int userId)
        {
            foreach (var attachment in attachments)
                attachment.UpdateModifiedFields(userId);

            return attachments;
        }
    }
}