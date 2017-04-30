using System.Threading.Tasks;
using SORANO.CORE.IdentityEntities;

namespace SORANO.BLL.Services.Abstract
{
    public interface IAccountService
    {
        Task<User> GetValidUser(string email, string password);

        Task<User> FindUserByEmail(string email);
    }
}