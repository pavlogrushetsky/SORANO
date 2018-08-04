using SORANO.BLL.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.BLL.Services.Abstract
{
    public interface IBaseService<T> where T : BaseDto
    {
        ServiceResponse<IEnumerable<T>> GetAll(bool withDeleted);

        Task<ServiceResponse<T>> GetAsync(int id);

        Task<ServiceResponse<int>> CreateAsync(T model, int userId);      

        Task<ServiceResponse<T>> UpdateAsync(T model, int userId);

        Task<ServiceResponse<int>> DeleteAsync(int id, int userId);
    }
}
