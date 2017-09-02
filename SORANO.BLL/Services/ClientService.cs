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
    public class ClientService : BaseService, IClientService
    {
        public ClientService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Client> CreateAsync(Client client, int userId)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client), Resource.ClientCannotBeNullException);
            }

            if (client.ID != 0)
            {
                throw new ArgumentException(Resource.ClientInvalidIdentifierException, nameof(client.ID));
            }

            var user = await _unitOfWork.Get<User>().GetAsync(s => s.ID == userId);

            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundMessage);
            }

            client.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            foreach (var recommendation in client.Recommendations)
            {
                recommendation.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            foreach (var attachment in client.Attachments)
            {
                attachment.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
            }

            var saved = _unitOfWork.Get<Client>().Add(client);

            await _unitOfWork.SaveAsync();

            return saved;
        }

        public async Task<IEnumerable<Client>> GetAllAsync(bool withDeleted)
        {
            var clients = await _unitOfWork.Get<Client>().GetAllAsync();

            if (!withDeleted)
            {
                return clients.Where(s => !s.IsDeleted);
            }

            return clients;
        }

        public async Task<Client> GetAsync(int id)
        {
            return await _unitOfWork.Get<Client>().GetAsync(s => s.ID == id);
        }

        public async Task<Client> GetIncludeAllAsync(int id)
        {
            var client = await _unitOfWork.Get<Client>().GetAsync(s => s.ID == id);

            return client;
        }

        public async Task<Client> UpdateAsync(Client client, int userId)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client), Resource.ClientCannotBeNullException);
            }

            if (client.ID <= 0)
            {
                throw new ArgumentException(Resource.ClientInvalidIdentifierException, nameof(client.ID));
            }

            var user = await _unitOfWork.Get<User>().GetAsync(u => u.ID == userId);

            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundMessage);
            }

            var existentClient = await _unitOfWork.Get<Client>().GetAsync(t => t.ID == client.ID);

            if (existentClient == null)
            {
                throw new ObjectNotFoundException(Resource.ClientNotFoundException);
            }

            existentClient.Name = client.Name;
            existentClient.Description = client.Description;
            existentClient.PhoneNumber = client.PhoneNumber;
            existentClient.CardNumber = client.CardNumber;

            existentClient.UpdateModifiedFields(userId);

            UpdateAttachments(client, existentClient, userId);

            UpdateRecommendations(client, existentClient, userId);

            var updated = _unitOfWork.Get<Client>().Update(existentClient);

            await _unitOfWork.SaveAsync();

            return updated;
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var existentClient = await _unitOfWork.Get<Client>().GetAsync(t => t.ID == id);

            if (existentClient.Goods.Any())
            {
                throw new Exception(Resource.ClientCannotBeDeletedException);
            }

            existentClient.UpdateDeletedFields(userId);

            _unitOfWork.Get<Client>().Update(existentClient);

            await _unitOfWork.SaveAsync();
        }
    }
}
