using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.CORE.AccountEntities;

namespace SORANO.BLL.Services.Abstract
{
    public interface IUserService
    {
        Task<List<User>> GetUsersAsync();

        Task<User> CreateAsync(User user);
    }
}