using SORANO.CORE.StockEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.BLL.Services.Abstract
{
    public interface IClientService
    {
        Task<IEnumerable<Client>> GetAllAsync(bool withDeleted);

        Task<Client> CreateAsync(Client client, int userId);

        Task<Client> GetAsync(int id);

        Task<Client> UpdateAsync(Client client, int userId);

        Task DeleteAsync(int id, int userId);

        Task<Client> GetIncludeAllAsync(int id);
    }
}
