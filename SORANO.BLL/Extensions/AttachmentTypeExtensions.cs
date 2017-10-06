using System.Linq;
using System.Text.RegularExpressions;
using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class AttachmentTypeExtensions
    {
        public static AttachmentTypeDto ToDto(this AttachmentType model)
        {
            var dto = new AttachmentTypeDto
            {
                ID = model.ID,
                Name = model.Name,
                Comment = model.Comment,
                Extensions = model.Extensions,
                AttachmentsCount = model.TypeAttachments.Count,
                CanBeUpdated = !model.Name.Equals("Основное изображение")
            };

            dto.MapDetails(model);
            dto.CanBeDeleted = model.Attachments.All(a => a.IsDeleted) && !model.IsDeleted && !model.Name.Equals("Основное изображение");

            return dto;
        }

        public static AttachmentType ToEntity(this AttachmentTypeDto dto)
        {
            return new AttachmentType
            {
                ID = dto.ID,
                Name = dto.Name,
                Comment = dto.Comment,
                Extensions = Regex.Replace(dto.Extensions.ToLower(), @"\s+", "")
            };
        }

        public static void UpdateFields(this AttachmentType existentAttachmentType, AttachmentType newAttachmentType)
        {
            existentAttachmentType.Name = newAttachmentType.Name;
            existentAttachmentType.Comment = newAttachmentType.Comment;
            existentAttachmentType.Extensions = newAttachmentType.Extensions;
        }
    }
}
