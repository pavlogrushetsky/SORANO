using System;
using System.Collections.Generic;
using System.Linq;
using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;

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
            dto.Recommendations = model.Recommendations?
                .Where(r => !r.IsDeleted)
                .Select(r => r.ToDto());
            dto.Attachments = model.Attachments?
                .Where(a => !a.IsDeleted && !a.Type.Name.Equals("Основное изображение"))
                .Select(a => a.ToDto());
            dto.MainPicture = model.Attachments?
                .Where(a => !a.IsDeleted && a.Type.Name.Equals("Основное изображение"))
                .OrderByDescending(a => a.ModifiedDate)
                .FirstOrDefault()?
                .ToDto() ?? new AttachmentDto();
        }

        public static string GetMainPicturePath(this StockEntity entity)
        {
            return entity.Attachments?
                .Where(a => !a.IsDeleted && a.Type.Name.Equals("Основное изображение"))
                .OrderByDescending(a => a.ModifiedDate)
                .FirstOrDefault()?.FullPath;
        }

        public static bool ContainsIgnoreCase(this string source, string toCheck)
        {
            return source != null && toCheck != null && source.IndexOf(toCheck, StringComparison.InvariantCultureIgnoreCase) >= 0;
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

        public static StockEntity UpdateRecommendations(this StockEntity to, StockEntity from, IUnitOfWork uow, int userId)
        {
            // Remove deleted recommendations for existent entity
            to.Recommendations
                .Where(r => !from.Recommendations.Select(x => x.ID).Contains(r.ID))
                .ToList()
                .ForEach(r =>
                {
                    r.UpdateDeletedFields(userId);
                    uow.Get<Recommendation>().Update(r);
                });

            // Update existent recommendations
            from.Recommendations
                .Where(r => to.Recommendations.Select(x => x.ID).Contains(r.ID))
                .ToList()
                .ForEach(r =>
                {
                    var rec = to.Recommendations.SingleOrDefault(x => x.ID == r.ID);
                    if (rec == null)
                    {
                        return;
                    }
                    rec.Comment = r.Comment;
                    rec.Value = r.Value;
                    rec.UpdateModifiedFields(userId);
                });

            // Add newly created recommendations to existent entity
            from.Recommendations
                .Where(r => !to.Recommendations.Select(x => x.ID).Contains(r.ID))
                .ToList()
                .ForEach(r =>
                {
                    r.ParentEntityID = to.ID;
                    r.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
                    to.Recommendations.Add(r);
                });

            return to;
        }

        public static StockEntity UpdateAttachments(this StockEntity to, StockEntity from, IUnitOfWork uow, int userId)
        {
            // Remove deleted attachments for existent entity
            to.Attachments
                .Where(a => !from.Attachments.Select(x => x.ID).Contains(a.ID))
                .ToList()
                .ForEach(a =>
                {
                    a.ParentEntities.Remove(to);
                    uow.Get<Attachment>().Delete(a);
                });

            // Update existent attachments
            from.Attachments
                .Where(a => to.Attachments.Select(x => x.ID).Contains(a.ID))
                .ToList()
                .ForEach(a =>
                {
                    var att = to.Attachments.SingleOrDefault(x => x.ID == a.ID);
                    if (att == null)
                    {
                        return;
                    }
                    att.Name = a.Name;
                    att.Description = a.Description;
                    att.FullPath = a.FullPath;
                    att.AttachmentTypeID = a.AttachmentTypeID;
                    att.UpdateModifiedFields(userId);
                });

            // Add newly created attachments to existent entity
            from.Attachments
                .Where(a => !to.Attachments.Select(x => x.ID).Contains(a.ID))
                .ToList()
                .ForEach(a =>
                {
                    a.ParentEntities.Add(to);
                    a.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
                    to.Attachments.Add(a);
                });

            return to;
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