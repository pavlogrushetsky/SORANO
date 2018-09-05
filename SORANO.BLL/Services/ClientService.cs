using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Linq;
using SORANO.BLL.Extensions;
using SORANO.BLL.Dtos;
using System;

namespace SORANO.BLL.Services
{
    public class ClientService : BaseService, IClientService
    {
        public ClientService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #region CRUD methods

        public ServiceResponse<IEnumerable<ClientDto>> GetAll(bool withDeleted)
        {
            var clients = UnitOfWork.Get<Client>()
                .GetAll(c => withDeleted || !c.IsDeleted)
                .OrderByDescending(c => c.ModifiedDate)
                .Select(c => new ClientDto
                {
                    ID = c.ID,
                    Name = c.Name,
                    Description = c.Description,
                    PhoneNumber = c.PhoneNumber,
                    CardNumber = c.CardNumber,
                    Modified = c.ModifiedDate,
                    CanBeDeleted = !c.IsDeleted && !c.Sales.Any(),
                    IsDeleted = c.IsDeleted
                })
                .ToList();

            return new SuccessResponse<IEnumerable<ClientDto>>(clients);
        }

        public async Task<ServiceResponse<ClientDto>> GetAsync(int id)
        {
            var client = await UnitOfWork.Get<Client>().GetAsync(s => s.ID == id, c => c.Sales);

            if (client == null)
                return new ServiceResponse<ClientDto>(ServiceResponseStatus.NotFound);

            client.Attachments = GetAttachments(id).ToList();
            client.Recommendations = GetRecommendations(id).ToList();

            return new SuccessResponse<ClientDto>(client.ToDto());
        }

        public async Task<ServiceResponse<int>> CreateAsync(ClientDto client, int userId)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            var entity = client.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Recommendations.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            entity.Attachments.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var added = UnitOfWork.Get<Client>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(added.ID);
        }       

        public async Task<ServiceResponse<ClientDto>> UpdateAsync(ClientDto client, int userId)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            var existentEntity = await UnitOfWork.Get<Client>().GetAsync(t => t.ID == client.ID);

            if (existentEntity == null)
                return new ServiceResponse<ClientDto>(ServiceResponseStatus.NotFound);

            var entity = client.ToEntity();

            existentEntity.Attachments = GetAttachments(existentEntity.ID).ToList();
            existentEntity.Recommendations = GetRecommendations(existentEntity.ID).ToList();

            existentEntity
                .UpdateFields(entity)
                .UpdateAttachments(entity, UnitOfWork, userId)
                .UpdateRecommendations(entity, UnitOfWork, userId)
                .UpdateModifiedFields(userId);        

            UnitOfWork.Get<Client>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<ClientDto>();
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            var existentClient = await UnitOfWork.Get<Client>().GetAsync(t => t.ID == id);

            if (existentClient == null)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            if (existentClient.Sales.Any())
                return new ServiceResponse<int>(ServiceResponseStatus.InvalidOperation);

            existentClient.UpdateDeletedFields(userId);

            UnitOfWork.Get<Client>().Update(existentClient);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        public ServiceResponse<IEnumerable<ClientDto>> GetAll(bool withDeleted, string searchTerm)
        {
            var term = searchTerm?.ToLower();
            var clients = UnitOfWork.Get<Client>()
                .GetAll(c =>
                    (term == null || c.Name.ToLower().Contains(term) ||
                     c.Description != null && c.Description.ToLower().Contains(term)) &&
                    (withDeleted || !c.IsDeleted), 
                    c => c.Sales)
                .OrderByDescending(c => c.ModifiedDate)
                .ToList()
                .Select(c => c.ToDto());

            return new SuccessResponse<IEnumerable<ClientDto>>(clients);
        }

        #endregion
    }
}
