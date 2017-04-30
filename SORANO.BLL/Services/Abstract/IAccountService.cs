using System.Threading.Tasks;
using SORANO.CORE.IdentityEntities;

namespace SORANO.BLL.Services.Abstract
{
    public interface IAccountService
    {
        Task<bool> IsUserValid(string email, string password);

        Task<User> FindUserByEmail(string email);
    }
}