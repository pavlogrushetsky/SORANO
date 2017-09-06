using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IAttachmentTypeService
    {
        Task<ServiceResponse<IEnumerable<AttachmentTypeDto>>> GetAllAsync(bool includeMainPicture);

        Task<ServiceResponse<int>> GetMainPictureTypeIdAsync();

        Task<ServiceResponse<AttachmentTypeDto>> GetAsync(int id);

        Task<ServiceResponse<AttachmentTypeDto>> CreateAsync(AttachmentTypeDto attachmentType, int userId);

        Task<ServiceResponse<AttachmentTypeDto>> UpdateAsync(AttachmentTypeDto attachmentType, int userId);

        Task<ServiceResponse<bool>> DeleteAsync(int id, int userId);

        Task<ServiceResponse<bool>> Exists(string name, int attachmentTypeId = 0);
    }
}
