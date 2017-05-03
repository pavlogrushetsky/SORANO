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

        public async Task<User> FindUserByLoginAsync(string login)
        {
            return await _userRepository.GetAsync(u => u.Login.Equals(login));
        }

        public async Task<User> FindUserByIdAsync(int id)
        {
            return await _userRepository.GetAsync(u => u.ID == id);
        }

        public async Task<User> GetUserAsync(string login, string password)
        {
            var hash = CryptoHelper.Hash(password);

            return await _userRepository.GetAsync(u => !u.IsBlocked && u.Login.Equals(login) && u.Password.Equals(hash), u => u.Roles);
        }

        public async Task ChangePasswordAsync(string login, string newPassword)
        {
            var user = await _userRepository.GetAsync(u => u.Login.Equals(login));

            user.Password = CryptoHelper.Hash(newPassword);

            await _userRepository.UpdateAsync(user);
        }
    }
}