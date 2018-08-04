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
            var response = new SuccessResponse<IEnumerable<ClientDto>>();

            var clients = UnitOfWork.Get<Client>().GetAll(c => c.Sales);

            var orderedClients = clients.OrderByDescending(c => c.ModifiedDate);

            response.Result = !withDeleted
                ? orderedClients.Where(c => !c.IsDeleted).Select(c => c.ToDto())
                : orderedClients.Select(c => c.ToDto());

            return response;
        }

        public async Task<ServiceResponse<ClientDto>> GetAsync(int id)
        {
            var client = await UnitOfWork.Get<Client>().GetAsync(s => s.ID == id);

            return client == null 
                ? new ServiceResponse<ClientDto>(ServiceResponseStatus.NotFound) 
                : new SuccessResponse<ClientDto>(client.ToDto());
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

            existentEntity.UpdateFields(entity);        
            existentEntity.UpdateModifiedFields(userId);

            UpdateAttachments(entity, existentEntity, userId);
            UpdateRecommendations(entity, existentEntity, userId);

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

        public async Task<ServiceResponse<IEnumerable<ClientDto>>> GetAllAsync(bool withDeleted, string searchTerm)
        {
            var response = new SuccessResponse<IEnumerable<ClientDto>>();

            var clients = UnitOfWork.Get<Client>().GetAll(c => c.Sales);

            var orderedClients = clients.OrderByDescending(c => c.ModifiedDate);

            var term = searchTerm?.ToLower();

            var searched = orderedClients
                .Where(l => string.IsNullOrEmpty(term)
                    || l.Name.ToLower().Contains(term)
                    || !string.IsNullOrWhiteSpace(l.Description) && l.Description.ToLower().Contains(term));

            if (withDeleted)
            {
                response.Result = searched.Select(t => t.ToDto());
                return response;
            }

            response.Result = searched.Where(t => !t.IsDeleted).Select(t => t.ToDto());
            return response;
        }

        #endregion
    }
}
