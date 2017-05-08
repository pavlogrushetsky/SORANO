using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.CORE.AccountEntities;

namespace SORANO.BLL.Services.Abstract
{
    public interface IUserService
    {
        Task<User> GetAsync(string login, string password);

        Task<User> GetAsync(string login);

        Task<User> GetIncludeAllAsync(int id);

        Task<User> GetAsync(int id);

        Task<User> GetIncludeRolesAsync(int id);

        Task<List<User>> GetAllIncludeAllAsync();

        Task<User> CreateAsync(User user);

        Task ChangePasswordAsync(string login, string newPassword);

        Task DeleteAsync(int id);

        Task<User> UpdateAsync(User user);
    }
}