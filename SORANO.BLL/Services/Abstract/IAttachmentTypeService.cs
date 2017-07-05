using SORANO.CORE.StockEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.BLL.Services.Abstract
{
    public interface IAttachmentTypeService
    {
        Task<IEnumerable<AttachmentType>> GetAllAsync();

        Task<int> GetMainPictureTypeIDAsync();

        Task<AttachmentType> GetAsync(int id);

        Task<AttachmentType> CreateAsync(AttachmentType attachmentType, int userId);

        Task<AttachmentType> UpdateAsync(AttachmentType attachmentType, int userId);

        Task DeleteAsync(int id, int userId);
    }
}
