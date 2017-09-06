using System;
using System.Linq;
using SORANO.BLL.DTOs;
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
                .Where(a => a.IsDeleted && !a.Type.Name.Equals("Основное изображение"))
                .Select(a => a.ToDto());
            dto.MainPicture = model.Attachments?
                                  .SingleOrDefault(a => !a.IsDeleted && a.Type.Name.Equals("Основное изображение"))?
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
    }
}