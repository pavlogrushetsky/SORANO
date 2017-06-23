using SORANO.CORE.StockEntities;
using SORANO.WEB.Models.AttachmentType;
using System.Linq;

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
                AttachmentsCount = attachmentType.Attachments.Count,
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
                Comment = model.Comment
            };
        }
    }
}
