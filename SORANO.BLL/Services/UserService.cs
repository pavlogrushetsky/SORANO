using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.AccountEntities;
using SORANO.BLL.Helpers;
using SORANO.DAL.Repositories;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }        

        /// <summary>
        /// Get all users including all related data
        /// </summary>
        /// <returns>List of users with all related data</returns>
        public async Task<List<User>> GetAllIncludeAllAsync()
        {
            var users = await _unitOfWork.Get<User>().GetAllAsync();

            return users.ToList();       
        }

        public async Task<User> GetIncludeRolesAsync(int id)
        {
            return await _unitOfWork.Get<User>().GetAsync(u => u.ID == id);           
        }

        public async Task<User> CreateAsync(User user)
        {
            var existentUser = await _unitOfWork.Get<User>().GetAsync(u => u.Login.Equals(user.Login));

            if (existentUser != null)
            {
                return null;
            }

            user.Password = CryptoHelper.Hash(user.Password);

            var added = _unitOfWork.Get<User>().Add(user);

            await _unitOfWork.SaveAsync();

            return added;          
        }

        public async Task DeleteAsync(int id)
        {
            var existentUser = await _unitOfWork.Get<User>().GetAsync(u => u.ID == id);

            _unitOfWork.Get<User>().Delete(existentUser);

            await _unitOfWork.SaveAsync();           
        }

        public async Task<User> GetAsync(string login)
        {
            return await _unitOfWork.Get<User>().GetAsync(u => u.Login.Equals(login));           
        }

        public async Task<User> GetIncludeAllAsync(int id)
        {
            var user = await _unitOfWork.Get<User>().GetAsync(u => u.ID == id);

            user.SoldGoods.ToList().ForEach(g =>
            {
                g.DeliveryItem = _unitOfWork.Get<DeliveryItem>().Get(d => d.ID == g.DeliveryItemID);
            });

            return user;           
        }

        public async Task<User> GetAsync(int id)
        {
            return await _unitOfWork.Get<User>().GetAsync(u => u.ID == id);         
        }

        public async Task<User> GetAsync(string login, string password)
        {
            var hash = CryptoHelper.Hash(password);

            return await _unitOfWork.Get<User>().GetAsync(u => !u.IsBlocked && u.Login.Equals(login) && u.Password.Equals(hash));          
        }

        public async Task ChangePasswordAsync(string login, string newPassword)
        {
            var user = await _unitOfWork.Get<User>().GetAsync(u => u.Login.Equals(login));

            user.Password = CryptoHelper.Hash(newPassword);

            _unitOfWork.Get<User>().Update(user);

            await _unitOfWork.SaveAsync();           
        }

        public async Task<User> UpdateAsync(User user)
        {
            var existentUser = await _unitOfWork.Get<User>().GetAsync(user.ID);

            existentUser.Login = user.Login;
            existentUser.Description = user.Description;

            if (!string.IsNullOrEmpty(user.Password))
            {
                existentUser.Password = CryptoHelper.Hash(user.Password);
            }

            var roles = await _unitOfWork.Get<Role>().GetAllAsync();

            existentUser.Roles
                .ToList()
                .Where(r => !user.Roles.Select(er => er.ID).Contains(r.ID)).ToList()
                .ForEach(r =>
                {
                    existentUser.Roles.Remove(r);
                });

            roles
                .Where(r => user.Roles.Select(er => er.ID).Contains(r.ID) &&
                            !existentUser.Roles.Select(er => er.ID).Contains(r.ID)).ToList()
                .ForEach(r =>
                {
                    existentUser.Roles.Add(r);
                });

            existentUser.Roles = user.Roles;

            var updated = _unitOfWork.Get<User>().Update(existentUser);

            await _unitOfWork.SaveAsync();

            return updated;           
        }
    }
}