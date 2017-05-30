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
        private readonly IDeliveryItemRepository _deliveryItemRepository;

        public UserService(IUserRepository userRepository, IDeliveryItemRepository deliveryItemRepository)
        {
            _userRepository = userRepository;
            _deliveryItemRepository = deliveryItemRepository;
        }        

        /// <summary>
        /// Get all users including all related data
        /// </summary>
        /// <returns>List of users with all related data</returns>
        public async Task<List<User>> GetAllIncludeAllAsync()
        {
            var users = await _userRepository.GetAllAsync(u => u.Roles, u => u.CreatedEntities,
                u => u.DeletedEntities,
                u => u.ModifiedEntities,
                u => u.SoldGoods);

            return users.ToList();
        }

        public async Task<User> GetIncludeRolesAsync(int id)
        {
            return await _userRepository.GetAsync(u => u.ID == id, u => u.Roles);
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

        public async Task DeleteAsync(int id)
        {
            var existentUser = await _userRepository.GetAsync(u => u.ID == id);

            await _userRepository.DeleteAsync(existentUser);
        }

        public async Task<User> GetAsync(string login)
        {
            return await _userRepository.GetAsync(u => u.Login.Equals(login));
        }

        public async Task<User> GetIncludeAllAsync(int id)
        {
            var user = await _userRepository.GetAsync(u => u.ID == id, u => u.Roles,
                u => u.CreatedEntities,
                u => u.DeletedEntities,
                u => u.ModifiedEntities,
                u => u.SoldGoods);

            user.SoldGoods.ToList().ForEach(g =>
            {
                g.DeliveryItem = _deliveryItemRepository.Get(d => d.ID == g.DeliveryItemID, d => d.Article);
            });

            return user;
        }

        public async Task<User> GetAsync(int id)
        {
            return await _userRepository.GetAsync(u => u.ID == id);
        }

        public async Task<User> GetAsync(string login, string password)
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

        public async Task<User> UpdateAsync(User user)
        {
            user.Password = CryptoHelper.Hash(user.Password);

            return await _userRepository.UpdateAsync(user);
        }
    }
}