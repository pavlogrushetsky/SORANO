using System.Threading.Tasks;
using SORANO.CORE.AccountEntities;

namespace SORANO.BLL.Services.Abstract
{
    public interface IAccountService
    {
        Task<User> GetUserAsync(string email, string password);

        Task<User> FindUserByEmailAsync(string email);

        Task<User> FindUserByIdAsync(int id);

        Task ChangePasswordAsync(string email, string newPassword);
    }
}