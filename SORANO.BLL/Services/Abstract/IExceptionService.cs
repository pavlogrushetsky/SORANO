using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IExceptionService
    {
        Task<ServiceResponse<bool>> SaveAsync(ExceptionDto exception);

        ServiceResponse<bool> Save(ExceptionDto exception);
    }
}