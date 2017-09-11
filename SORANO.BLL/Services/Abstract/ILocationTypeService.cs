using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface ILocationTypeService : IBaseService<LocationTypeDto>
    {
        Task<ServiceResponse<bool>> Exists(string name, int? locationType, int userId);
    }
}
