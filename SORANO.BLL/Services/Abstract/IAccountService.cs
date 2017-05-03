using System.Threading.Tasks;
using SORANO.CORE.AccountEntities;

namespace SORANO.BLL.Services.Abstract
{
    public interface IAccountService
    {
        Task<User> GetUserAsync(string login, string password);

        Task<User> FindUserByLoginAsync(string login);

        Task<User> FindUserByIdAsync(int id);

        Task ChangePasswordAsync(string login, string newPassword);
    }
}