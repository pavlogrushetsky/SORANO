using SORANO.BLL.Dtos;
using SORANO.BLL.Extensions;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SORANO.BLL.Services
{
    public class SaleService : BaseService, ISaleService
    {
        public SaleService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }                             

        #region CRUD Methods

        public async Task<ServiceResponse<IEnumerable<SaleDto>>> GetAllAsync(bool withDeleted)
        {
            var response = new SuccessResponse<IEnumerable<SaleDto>>();

            var sales = await UnitOfWork.Get<Sale>().GetAllAsync();

            response.Result = !withDeleted
                ? sales.Where(s => !s.IsDeleted).Select(s => s.ToDto())
                : sales.Select(s => s.ToDto());

            return response;
        }

        public Task<ServiceResponse<SaleDto>> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<int>> CreateAsync(SaleDto article, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<SaleDto>> UpdateAsync(SaleDto article, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
