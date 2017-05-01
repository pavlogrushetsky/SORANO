using System.Threading.Tasks;
using SORANO.CORE.AccountEntities;

namespace SORANO.BLL.Services.Abstract
{
    public interface IAccountService
    {
        Task<User> GetUser(string email, string password);

        Task<User> FindUserByEmail(string email);
    }
}