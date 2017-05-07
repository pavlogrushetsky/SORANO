using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.CORE.AccountEntities;

namespace SORANO.BLL.Services.Abstract
{
    public interface IUserService
    {
        Task<User> GetAsync(string login, string password);

        Task<User> GetAsync(string login);

        Task<User> GetAsync(int id);

        Task<List<User>> GetAllAsync();

        Task<User> CreateAsync(User user);

        Task ChangePasswordAsync(string login, string newPassword);

        Task DeleteAsync(int id);
    }
}