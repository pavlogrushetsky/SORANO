using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IAttachmentService
    {
        ServiceResponse<IEnumerable<string>> GetAllFor(string type);

        ServiceResponse<IEnumerable<AttachmentDto>> GetPicturesExcept(int currentMainPictureId);

        Task<ServiceResponse<bool>> HasMainPictureAsync(int id, int mainPictureId);
    }
}
