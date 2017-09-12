using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using SORANO.BLL.Properties;
using System.Linq;
using SORANO.BLL.Extensions;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services
{
    public class SupplierService : BaseService, ISupplierService
    {
        public SupplierService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<ServiceResponse<SupplierDto>> CreateAsync(SupplierDto supplier, int userId)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            var entity = supplier.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Recommendations.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Attachments.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var saved = UnitOfWork.Get<Supplier>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<SupplierDto>(saved.ToDto());
        }

        public async Task<ServiceResponse<IEnumerable<SupplierDto>>> GetAllAsync(bool withDeleted)
        {
            var response = new SuccessResponse<IEnumerable<SupplierDto>>();

            var suppliers = await UnitOfWork.Get<Supplier>().GetAllAsync();

            response.Result = !withDeleted
                ? suppliers.Where(s => !s.IsDeleted).Select(s => s.ToDto())
                : suppliers.Select(a => a.ToDto());

            return response;
        }

        public async Task<ServiceResponse<SupplierDto>> GetAsync(int id)
        {
            var supplier = await UnitOfWork.Get<Supplier>().GetAsync(s => s.ID == id);

            if (supplier == null)
                return new FailResponse<SupplierDto>(Resource.SupplierNotFoundMessage);

            return new SuccessResponse<SupplierDto>(supplier.ToDto());
        }

        public async Task<ServiceResponse<SupplierDto>> UpdateAsync(SupplierDto supplier, int userId)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            var existentEntity = await UnitOfWork.Get<Supplier>().GetAsync(t => t.ID == supplier.ID);

            if (existentEntity == null)
                return new FailResponse<SupplierDto>(Resource.SupplierNotFoundMessage);

            var entity = supplier.ToEntity();

            existentEntity.UpdateFields(entity);
            existentEntity.UpdateModifiedFields(userId);

            UpdateAttachments(entity, existentEntity, userId);
            UpdateRecommendations(entity, existentEntity, userId);

            var updated = UnitOfWork.Get<Supplier>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<SupplierDto>(updated.ToDto());
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            var existentSupplier = await UnitOfWork.Get<Supplier>().GetAsync(t => t.ID == id);

            if (existentSupplier.Deliveries.Any())
                return new FailResponse<int>(Resource.SupplierCannotBeDeletedMessage);

            existentSupplier.UpdateDeletedFields(userId);

            UnitOfWork.Get<Supplier>().Update(existentSupplier);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }
    }
}
