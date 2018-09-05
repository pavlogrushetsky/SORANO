using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
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

        #region CRUD methods

        public ServiceResponse<IEnumerable<SupplierDto>> GetAll(bool withDeleted)
        {
            var suppliers = UnitOfWork.Get<Supplier>()
                .GetAll(s => withDeleted || !s.IsDeleted)
                .OrderByDescending(s => s.ModifiedDate)
                .Select(s => new SupplierDto
                {
                    ID = s.ID,
                    Name = s.Name,
                    Description = s.Description,
                    Modified = s.ModifiedDate,
                    CanBeDeleted = !s.IsDeleted && !s.Deliveries.Any(),
                    IsDeleted = s.IsDeleted
                })
                .ToList();

            return new SuccessResponse<IEnumerable<SupplierDto>>(suppliers);
        }

        public async Task<ServiceResponse<SupplierDto>> GetAsync(int id)
        {
            var supplier = await UnitOfWork.Get<Supplier>()
                .GetAsync(s => s.ID == id,
                    s => s.Deliveries);

            if (supplier == null)
                return new ServiceResponse<SupplierDto>(ServiceResponseStatus.NotFound);

            supplier.Attachments = GetAttachments(id).ToList();
            supplier.Recommendations = GetRecommendations(id).ToList();

            return new SuccessResponse<SupplierDto>(supplier.ToDto());
        }

        public async Task<ServiceResponse<int>> CreateAsync(SupplierDto supplier, int userId)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            var entity = supplier.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Recommendations.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Attachments.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var added = UnitOfWork.Get<Supplier>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(added.ID);
        }        

        public async Task<ServiceResponse<SupplierDto>> UpdateAsync(SupplierDto supplier, int userId)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            var existentEntity = await UnitOfWork.Get<Supplier>().GetAsync(t => t.ID == supplier.ID);

            if (existentEntity == null)
                return new ServiceResponse<SupplierDto>(ServiceResponseStatus.NotFound);

            var entity = supplier.ToEntity();

            existentEntity.Attachments = GetAttachments(existentEntity.ID).ToList();
            existentEntity.Recommendations = GetRecommendations(existentEntity.ID).ToList();

            existentEntity
                .UpdateFields(entity)
                .UpdateAttachments(entity, UnitOfWork, userId)
                .UpdateRecommendations(entity, UnitOfWork, userId)
                .UpdateModifiedFields(userId);

            UnitOfWork.Get<Supplier>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<SupplierDto>();
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            var existentSupplier = await UnitOfWork.Get<Supplier>().GetAsync(t => t.ID == id);

            if (existentSupplier == null)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            if (existentSupplier.Deliveries.Any())
                return new ServiceResponse<int>(ServiceResponseStatus.InvalidOperation);

            existentSupplier.UpdateDeletedFields(userId);

            UnitOfWork.Get<Supplier>().Update(existentSupplier);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        #endregion

        public ServiceResponse<IEnumerable<SupplierDto>> GetAll(bool withDeleted, string searchTerm)
        {
            var term = searchTerm?.ToLower();

            var suppliers = UnitOfWork.Get<Supplier>()
                .GetAll(s => (term == null || s.Name.ToLower().Contains(term) || s.Description != null && s.Description.ToLower().Contains(term)) && 
                (withDeleted || !s.IsDeleted), 
                s => s.Deliveries)
                .OrderByDescending(s => s.ModifiedDate)
                .ToList()
                .Select(s => s.ToDto());

            return new SuccessResponse<IEnumerable<SupplierDto>>(suppliers);
        }

        public ServiceResponse<SupplierDto> GetDefaultSupplier()
        {
            var supplier = UnitOfWork.Get<Supplier>()
                .GetAll(s => !s.IsDeleted, s => s.Deliveries)
                .ToList()
                .FirstOrDefault();

            return supplier == null 
                ? new ServiceResponse<SupplierDto>(ServiceResponseStatus.NotFound) 
                : new SuccessResponse<SupplierDto>(supplier.ToDto());
        }
    }
}
