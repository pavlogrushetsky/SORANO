﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Services.Abstract
{
    public interface IAttachmentService
    {
        Task<IEnumerable<string>> GetAllForAsync(string type);

        Task<IEnumerable<Attachment>> GetPicturesExceptAsync(int currentMainPictureId);

        Task<bool> HasMainPictureAsync(int id, int mainPictureId);
    }
}