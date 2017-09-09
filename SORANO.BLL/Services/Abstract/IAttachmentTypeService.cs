using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IAttachmentTypeService : IBaseService<AttachmentTypeDto>
    {
        Task<ServiceResponse<int>> GetMainPictureTypeIdAsync(int userId);

        Task<ServiceResponse<bool>> Exists(string name, int? attachmentTypeId, int userId);
    }
}
