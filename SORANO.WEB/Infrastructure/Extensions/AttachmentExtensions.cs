using SORANO.CORE.StockEntities;
using SORANO.WEB.Models.Attachment;
using System.IO;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class AttachmentExtensions
    {
        public static AttachmentModel ToModel(this Attachment attachment)
        {
            return new AttachmentModel
            {
                ID = attachment.ID,
                Name = attachment.Name,
                Description = attachment.Description,
                Type = attachment.Type.ToModel(),
                TypeID = attachment.AttachmentTypeID.ToString(),
                FullPath = attachment.FullPath,
                Extension = Path.GetExtension(attachment.Name)
            };
        }
    }
}
