using SORANO.BLL.DTOs;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class AttachmentTypeDtoExtensions
    {
        public static AttachmentTypeDto ToDto(this AttachmentType model)
        {
            return new AttachmentTypeDto
            {
                Name = model.Name,
                Comment = model.Comment,
                Extensions = model.Extensions,
                AttachmentsCount = model.TypeAttachments.Count
            };
        }
    }
}
