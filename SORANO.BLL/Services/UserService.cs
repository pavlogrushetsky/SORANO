using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.AccountEntities;
using SORANO.DAL.Repositories.Abstract;
using SORANO.BLL.Helpers;

namespace SORANO.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }        

        public async Task<List<User>> GetUsersAsync()
        {
            var users = await _userRepository.GetAllAsync(u => u.Roles);

            return users.ToList();
        }

        public async Task<User> CreateAsync(User user)
        {
            var existentUser = await _userRepository.GetAsync(u => u.Login.Equals(user.Login));

            if (existentUser != null)
            {
                return null;
            }

            user.Password = CryptoHelper.Hash(user.Password);

            return await _userRepository.AddAsync(user);
        }
    }
}