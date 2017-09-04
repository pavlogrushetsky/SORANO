﻿using SORANO.BLL.DTOs;
using SORANO.CORE.StockEntities;
using System.IO;

namespace SORANO.BLL.Extensions
{
    internal static class AttachmentDtoExtensions
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
                AttachmentType = model.Type.ToDto()
            };
        }
    }
}
