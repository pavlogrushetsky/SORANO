using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IAttachmentService
    {
        ServiceResponse<IEnumerable<string>> GetAllFor(string type);

        Task<ServiceResponse<IEnumerable<AttachmentDto>>> GetPicturesExceptAsync(int currentMainPictureId);

        Task<ServiceResponse<bool>> HasMainPictureAsync(int id, int mainPictureId);
    }
}
