using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;
using System.IO;

namespace SORANO.BLL.Extensions
{
    internal static class AttachmentExtensions
    {
        public static AttachmentDto ToDto(this Attachment model)
        {
            return new AttachmentDto
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                FullPath = model.FullPath,
                Extension = Path.GetExtension(model.Name),
                AttachmentTypeID = model.AttachmentTypeID,
                AttachmentType = model.Type?.ToDto()
            };
        }

        public static Attachment ToEntity(this AttachmentDto dto)
        {
            return new Attachment
            {
                ID = dto.ID,
                Name = dto.Name,
                Description = dto.Description,
                FullPath = dto.FullPath,
                AttachmentTypeID = dto.AttachmentTypeID
            };
        }
    }
}
