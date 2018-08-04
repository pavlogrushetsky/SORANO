using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IAttachmentTypeService : IBaseService<AttachmentTypeDto>
    {
        Task<ServiceResponse<int>> GetMainPictureTypeIdAsync(int userId);

        ServiceResponse<IEnumerable<AttachmentTypeDto>> GetAll(string searchTerm);       
    }
}
