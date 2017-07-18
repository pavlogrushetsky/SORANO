using SORANO.CORE.StockEntities;
using System.IO;
using SORANO.WEB.Models;

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

        public static Attachment ToEntity(this AttachmentModel model)
        {
            return new Attachment
            {
                ID = model.ID,
                Name = model.Name,
                FullPath = model.FullPath,
                Description = model.Description,
                AttachmentTypeID = int.Parse(model.TypeID)
            };
        }
    }
}
