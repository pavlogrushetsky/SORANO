using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IAttachmentService
    {
        Task<ServiceResponse<IEnumerable<string>>> GetAllForAsync(string type, int userId);

        Task<ServiceResponse<IEnumerable<AttachmentDto>>> GetPicturesExceptAsync(int currentMainPictureId, int userId);

        Task<ServiceResponse<bool>> HasMainPictureAsync(int id, int mainPictureId, int userId);
    }
}
