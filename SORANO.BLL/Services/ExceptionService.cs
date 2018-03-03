using System;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;
using SORANO.BLL.Extensions;
using SORANO.BLL.Services.Abstract;
using SORANO.DAL.Repositories;

namespace SORANO.BLL.Services
{
    public class ExceptionService : IExceptionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExceptionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<bool>> SaveAsync(ExceptionDto exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            var ex = exception.ToEntity();

            _unitOfWork.Get<CORE.Exception>().Add(ex);

            await _unitOfWork.SaveAsync();

            return new SuccessResponse<bool>(true);
        }

        public ServiceResponse<bool> Save(ExceptionDto exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            var ex = exception.ToEntity();

            _unitOfWork.Get<CORE.Exception>().Add(ex);

            _unitOfWork.Save();

            return new SuccessResponse<bool>(true);
        }
    }
}