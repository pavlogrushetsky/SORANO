﻿using System.Collections.Generic;
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
            var users = await _unitOfWork.Get<User>().GetAllAsync(u => u.Roles, u => u.CreatedEntities,
            u => u.DeletedEntities,
            u => u.ModifiedEntities,
            u => u.SoldGoods);

            return users.ToList();       
        }

        public async Task<User> GetIncludeRolesAsync(int id)
        {
            return await _unitOfWork.Get<User>().GetAsync(u => u.ID == id, u => u.Roles);           
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
            var user = await _unitOfWork.Get<User>().GetAsync(u => u.ID == id, u => u.Roles,
            u => u.CreatedEntities,
            u => u.DeletedEntities,
            u => u.ModifiedEntities,
            u => u.SoldGoods);

            user.SoldGoods.ToList().ForEach(g =>
            {
                g.DeliveryItem = _unitOfWork.Get<DeliveryItem>().Get(d => d.ID == g.DeliveryItemID, d => d.Article);
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

            return await _unitOfWork.Get<User>().GetAsync(u => !u.IsBlocked && u.Login.Equals(login) && u.Password.Equals(hash), u => u.Roles);          
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
            user.Password = CryptoHelper.Hash(user.Password);

            var updated = _unitOfWork.Get<User>().Update(user);

            await _unitOfWork.SaveAsync();

            return updated;           
        }
    }
}