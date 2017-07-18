using SORANO.CORE.StockEntities;
using System.Linq;
using System.Text.RegularExpressions;
using SORANO.WEB.Models;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class AttachmentTypeExtensions
    {
        public static AttachmentTypeModel ToModel(this AttachmentType attachmentType)
        {
            return new AttachmentTypeModel
            {
                ID = attachmentType.ID,
                Name = attachmentType.Name,
                Comment = attachmentType.Comment,               
                Extensions = attachmentType.Extensions,
                MimeTypes = !string.IsNullOrEmpty(attachmentType.Extensions) ? string.Join(",", attachmentType.Extensions.Split(',').Select(MimeTypes.MimeTypeMap.GetMimeType)) : "",
                AttachmentsCount = attachmentType.TypeAttachments.Count,
                CanBeDeleted = !attachmentType.Attachments.Any() && !attachmentType.IsDeleted && !attachmentType.Name.Equals("Основное изображение"),
                IsDeleted = attachmentType.IsDeleted
            };
        }

        public static AttachmentType ToEntity(this AttachmentTypeModel model)
        {
            return new AttachmentType
            {
                ID = model.ID,
                Name = model.Name,
                Comment = model.Comment,
                Extensions = Regex.Replace(model.Extensions.ToLower(), @"\s+", "")
            };
        }
    }
}
