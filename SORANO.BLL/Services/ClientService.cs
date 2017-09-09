using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using SORANO.BLL.Properties;
using System.Data;
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

        public async Task<ServiceResponse<ClientDto>> CreateAsync(ClientDto client, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<ClientDto>();

            if (client == null)
                throw new ArgumentNullException(nameof(client));

            var entity = client.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            foreach (var recommendation in entity.Recommendations)
            {
                recommendation.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            foreach (var attachment in entity.Attachments)
            {
                attachment.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            var saved = UnitOfWork.Get<Client>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<ClientDto>(saved.ToDto());
        }

        public async Task<ServiceResponse<IEnumerable<ClientDto>>> GetAllAsync(bool withDeleted, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<IEnumerable<ClientDto>>();

            var response = new SuccessResponse<IEnumerable<ClientDto>>();

            var clients = await UnitOfWork.Get<Client>().GetAllAsync();

            response.Result = !withDeleted
                ? clients.Where(c => !c.IsDeleted).Select(c => c.ToDto())
                : clients.Select(c => c.ToDto());

            return response;
        }

        public async Task<ServiceResponse<ClientDto>> GetAsync(int id, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<ClientDto>();

            var client = await UnitOfWork.Get<Client>().GetAsync(s => s.ID == id);

            if (client == null)
                return new FailResponse<ClientDto>(Resource.ClientNotFoundMessage);

            return new SuccessResponse<ClientDto>(client.ToDto());
        }

        public async Task<ServiceResponse<ClientDto>> UpdateAsync(ClientDto client, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<ClientDto>();

            if (client == null)
                throw new ArgumentNullException(nameof(client));

            var existentEntity = await UnitOfWork.Get<Client>().GetAsync(t => t.ID == client.ID);

            if (existentEntity == null)
                return new FailResponse<ClientDto>(Resource.ClientNotFoundMessage);

            var entity = client.ToEntity();

            existentEntity.UpdateFields(entity);        
            existentEntity.UpdateModifiedFields(userId);

            UpdateAttachments(entity, existentEntity, userId);
            UpdateRecommendations(entity, existentEntity, userId);

            var updated = UnitOfWork.Get<Client>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<ClientDto>(updated.ToDto());
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<int>();

            var existentClient = await UnitOfWork.Get<Client>().GetAsync(t => t.ID == id);

            if (existentClient.Goods.Any())
                return new FailResponse<int>(Resource.ClientCannotBeDeletedMessage);

            existentClient.UpdateDeletedFields(userId);

            UnitOfWork.Get<Client>().Update(existentClient);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }
    }
}
