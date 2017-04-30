using System.Threading.Tasks;
using SORANO.BLL.Helpers;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.IdentityEntities;
using SORANO.DAL.Repositories.Abstract;

namespace SORANO.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;

        public AccountService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> FindUserByEmail(string email)
        {
            return await _userRepository.GetAsync(u => u.Email.Equals(email));
        }

        public async Task<bool> IsUserValid(string email, string password)
        {
            var hash = CryptoHelper.Hash(password);

            var user = await _userRepository.GetAsync(u => u.Email.Equals(email) && u.Password.Equals(hash));

            return user != null;
        }
    }
}