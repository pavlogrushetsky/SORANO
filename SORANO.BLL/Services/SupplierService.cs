using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using SORANO.BLL.Properties;
using SORANO.CORE.AccountEntities;
using System.Data;
using SORANO.BLL.Helpers;
using System.Linq;

namespace SORANO.BLL.Services
{
    public class SupplierService : BaseService, ISupplierService
    {
        public SupplierService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Supplier> CreateAsync(Supplier supplier, int userId)
        {
            // Check passed supplier
            if (supplier == null)
            {
                throw new ArgumentNullException(nameof(supplier), Resource.SupplierCannotBeNullException);
            }

            // Identifier of new supplier must be equal 0
            if (supplier.ID != 0)
            {
                throw new ArgumentException(Resource.SupplierInvalidIdentifierException, nameof(supplier.ID));
            }

            // Get user by specified identifier
            var user = await _unitOfWork.Get<User>().GetAsync(s => s.ID == userId);

            // Check user
            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundException);
            }

            // Update created and modified fields for supplier
            supplier.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            // Update created and modified fields for each supplier recommendation
            foreach (var recommendation in supplier.Recommendations)
            {
                recommendation.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            var saved = _unitOfWork.Get<Supplier>().Add(supplier);

            await _unitOfWork.SaveAsync();

            return saved;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            return await _unitOfWork.Get<Supplier>().GetAllAsync();
        }

        public async Task<Supplier> GetAsync(int id)
        {
            return await _unitOfWork.Get<Supplier>().GetAsync(s => s.ID == id);
        }

        public async Task<Supplier> GetIncludeAllAsync(int id)
        {
            var supplier = await _unitOfWork.Get<Supplier>().GetAsync(s => s.ID == id);

            return supplier;
        }

        public async Task<Supplier> UpdateAsync(Supplier supplier, int userId)
        {
            // Check passed supplier
            if (supplier == null)
            {
                throw new ArgumentNullException(nameof(supplier), Resource.SupplierCannotBeNullException);
            }

            // Identifier of supplier must be > 0
            if (supplier.ID <= 0)
            {
                throw new ArgumentException(Resource.SupplierInvalidIdentifierException, nameof(supplier.ID));
            }

            // Get user by specified identifier
            var user = await _unitOfWork.Get<User>().GetAsync(u => u.ID == userId);

            // Check user
            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundException);
            }

            // Get existent supplier by identifier
            var existentSupplier = await _unitOfWork.Get<Supplier>().GetAsync(t => t.ID == supplier.ID);

            // Check existent supplier
            if (supplier == null)
            {
                throw new ObjectNotFoundException(Resource.SupplierNotFoundException);
            }

            // Update fields
            existentSupplier.Name = supplier.Name;
            existentSupplier.Description = supplier.Description;

            // Update modified fields for existent article
            existentSupplier.UpdateModifiedFields(userId);

            UpdateRecommendations(supplier, existentSupplier, userId);

            var updated = _unitOfWork.Get<Supplier>().Update(existentSupplier);

            await _unitOfWork.SaveAsync();

            return updated;
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var existentSupplier = await _unitOfWork.Get<Supplier>().GetAsync(t => t.ID == id);

            if (existentSupplier.Deliveries.Any())
            {
                throw new Exception(Resource.SupplierCannotBeDeletedException);
            }

            existentSupplier.UpdateDeletedFields(userId);

            _unitOfWork.Get<Supplier>().Update(existentSupplier);

            await _unitOfWork.SaveAsync();
        }
    }
}
