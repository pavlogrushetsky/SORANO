using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using SORANO.BLL.Properties;
using SORANO.CORE.AccountEntities;
using System.Data;
using System.Linq;
using SORANO.BLL.Extensions;

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
                throw new ArgumentNullException(nameof(supplier), Resource.SupplierCannotBeNullMessage);
            }

            // Identifier of new supplier must be equal 0
            if (supplier.ID != 0)
            {
                throw new ArgumentException(Resource.SupplierInvalidIdentifierMessage, nameof(supplier.ID));
            }

            // Get user by specified identifier
            var user = await UnitOfWork.Get<User>().GetAsync(s => s.ID == userId);

            // Check user
            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundMessage);
            }

            // Update created and modified fields for supplier
            supplier.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            // Update created and modified fields for each supplier recommendation
            foreach (var recommendation in supplier.Recommendations)
            {
                recommendation.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            foreach (var attachment in supplier.Attachments)
            {
                attachment.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            var saved = UnitOfWork.Get<Supplier>().Add(supplier);

            await UnitOfWork.SaveAsync();

            return saved;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync(bool withDeleted)
        {
            var suppliers = await UnitOfWork.Get<Supplier>().GetAllAsync();

            if (!withDeleted)
            {
                return suppliers.Where(s => !s.IsDeleted);
            }

            return suppliers;
        }

        public async Task<Supplier> GetAsync(int id)
        {
            return await UnitOfWork.Get<Supplier>().GetAsync(s => s.ID == id);
        }

        public async Task<Supplier> GetIncludeAllAsync(int id)
        {
            var supplier = await UnitOfWork.Get<Supplier>().GetAsync(s => s.ID == id);

            return supplier;
        }

        public async Task<Supplier> UpdateAsync(Supplier supplier, int userId)
        {
            // Check passed supplier
            if (supplier == null)
            {
                throw new ArgumentNullException(nameof(supplier), Resource.SupplierCannotBeNullMessage);
            }

            // Identifier of supplier must be > 0
            if (supplier.ID <= 0)
            {
                throw new ArgumentException(Resource.SupplierInvalidIdentifierMessage, nameof(supplier.ID));
            }

            // Get user by specified identifier
            var user = await UnitOfWork.Get<User>().GetAsync(u => u.ID == userId);

            // Check user
            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundMessage);
            }

            // Get existent supplier by identifier
            var existentSupplier = await UnitOfWork.Get<Supplier>().GetAsync(t => t.ID == supplier.ID);

            // Check existent supplier
            if (existentSupplier == null)
            {
                throw new ObjectNotFoundException(Resource.SupplierNotFoundMessage);
            }

            // Update fields
            existentSupplier.Name = supplier.Name;
            existentSupplier.Description = supplier.Description;

            // Update modified fields for existent article
            existentSupplier.UpdateModifiedFields(userId);

            UpdateAttachments(supplier, existentSupplier, userId);

            UpdateRecommendations(supplier, existentSupplier, userId);

            var updated = UnitOfWork.Get<Supplier>().Update(existentSupplier);

            await UnitOfWork.SaveAsync();

            return updated;
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var existentSupplier = await UnitOfWork.Get<Supplier>().GetAsync(t => t.ID == id);

            if (existentSupplier.Deliveries.Any())
            {
                throw new Exception(Resource.SupplierCannotBeDeletedMessage);
            }

            existentSupplier.UpdateDeletedFields(userId);

            UnitOfWork.Get<Supplier>().Update(existentSupplier);

            await UnitOfWork.SaveAsync();
        }
    }
}
