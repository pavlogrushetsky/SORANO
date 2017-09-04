using SORANO.CORE.StockEntities;
using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.DTOs;

namespace SORANO.BLL.Services.Abstract
{
    public interface IAttachmentTypeService
    {
        Task<IEnumerable<AttachmentTypeDto>> GetAllAsync(bool includeMainPicture);

        Task<int> GetMainPictureTypeIDAsync();

        Task<AttachmentType> GetAsync(int id);

        Task<AttachmentType> CreateAsync(AttachmentType attachmentType, int userId);

        Task<AttachmentType> UpdateAsync(AttachmentType attachmentType, int userId);

        Task DeleteAsync(int id, int userId);

        Task<bool> Exists(string name, int attachmentTypeId = 0);
    }
}
