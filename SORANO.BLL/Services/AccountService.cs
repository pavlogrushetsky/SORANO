using System.Threading.Tasks;
using SORANO.BLL.Helpers;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.AccountEntities;
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

        public async Task<User> FindUserByEmailAsync(string email)
        {
            return await _userRepository.GetAsync(u => u.Email.Equals(email));
        }

        public async Task<User> FindUserByIdAsync(int id)
        {
            return await _userRepository.GetAsync(u => u.ID == id);
        }

        public async Task<User> GetUserAsync(string email, string password)
        {
            var hash = CryptoHelper.Hash(password);

            return await _userRepository.GetAsync(u => u.Email.Equals(email) && u.Password.Equals(hash), u => u.Roles);
        }

        public async Task ChangePasswordAsync(string email, string newPassword)
        {
            var user = await _userRepository.GetAsync(u => u.Email.Equals(email));

            user.Password = CryptoHelper.Hash(newPassword);

            await _userRepository.UpdateAsync(user);
        }
    }
}