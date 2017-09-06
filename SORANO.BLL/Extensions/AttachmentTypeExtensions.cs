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
    }
}
