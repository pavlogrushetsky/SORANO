using System.Text.RegularExpressions;
using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class AttachmentTypeExtensions
    {
        public static AttachmentTypeDto ToDto(this AttachmentType model)
        {
            return new AttachmentTypeDto
            {
                ID = model.ID,
                Name = model.Name,
                Comment = model.Comment,
                Extensions = model.Extensions,
                AttachmentsCount = model.TypeAttachments.Count
            };
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
