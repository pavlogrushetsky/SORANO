using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IAttachmentService
    {
        Task<ServiceResponse<IEnumerable<string>>> GetAllForAsync(string type);

        Task<ServiceResponse<IEnumerable<AttachmentDto>>> GetPicturesExceptAsync(int currentMainPictureId);

        Task<ServiceResponse<bool>> HasMainPictureAsync(int id, int mainPictureId);
    }
}
