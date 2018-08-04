using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;
using SORANO.BLL.Extensions;
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

        public ServiceResponse<IEnumerable<UserDto>> GetAll()
        {
            var users = _unitOfWork.Get<User>()
                .GetAll(u => u.Roles, u => u.Locations)
                .ToList();

            return new SuccessResponse<IEnumerable<UserDto>>(users.Select(u => u.ToDto()));       
        }

        public async Task<ServiceResponse<UserDto>> CreateAsync(UserDto user)
        {
            if (user == null)
                return new ServiceResponse<UserDto>(ServiceResponseStatus.InvalidOperation);

            var usersWithSameLogin = _unitOfWork.Get<User>()
                .GetAll(u => u.Login.Equals(user.Login) && 
                             u.ID != user.ID)
                .ToList();

            if (usersWithSameLogin.Any())
                return new ServiceResponse<UserDto>(ServiceResponseStatus.AlreadyExists);

            var entity = user.ToEntity();
            entity.Password = CryptoHelper.Hash(user.Password);

            var userRoleIds = user.Roles.Select(x => x.ID).ToList();
            entity.Roles = _unitOfWork.Get<Role>()
                .GetAll(r => userRoleIds.Contains(r.ID))
                .ToList();

            if (user.Locations != null && user.Locations.Any())
            {
                var userLocationIds = user.Locations.Select(l => l.ID);
                entity.Locations = _unitOfWork.Get<Location>()
                    .GetAll(l => userLocationIds.Contains(l.ID))
                    .ToList();
            }            

            _unitOfWork.Get<User>().Add(entity);

            await _unitOfWork.SaveAsync();

            return new SuccessResponse<UserDto>();          
        }

        public async Task<ServiceResponse<UserDto>> UpdateAsync(UserDto user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var existentUser = await _unitOfWork.Get<User>()
                .GetAsync(user.ID, 
                    u => u.Roles, 
                    u => u.Locations);

            if (existentUser == null)
                return new ServiceResponse<UserDto>(ServiceResponseStatus.NotFound);

            var usersWithSameLogin = _unitOfWork.Get<User>()
                .GetAll(u => u.Login.Equals(user.Login) && 
                             u.ID != user.ID)
                .ToList();

            if (usersWithSameLogin.Any())
                return new ServiceResponse<UserDto>(ServiceResponseStatus.AlreadyExists);

            existentUser.Login = user.Login;
            existentUser.Description = user.Description;
            existentUser.IsBlocked = user.IsBlocked;

            if (!string.IsNullOrEmpty(user.Password))
                existentUser.Password = CryptoHelper.Hash(user.Password);

            var roles = _unitOfWork.Get<Role>()
                .GetAll()
                .ToList();

            user.Roles.ToList().ForEach(r =>
            {
                if (!existentUser.Roles.Select(er => er.ID).Contains(r.ID))
                    existentUser.Roles.Add(roles.Single(x => x.ID == r.ID));
            });

            existentUser.Roles.ToList().ForEach(er =>
            {
                if (!user.Roles.Select(r => r.ID).Contains(er.ID))
                    existentUser.Roles.Remove(er);
            });

            if (user.Locations != null)
            {
                var locations = _unitOfWork.Get<Location>()
                    .GetAll()
                    .ToList();

                user.Locations.ToList().ForEach(l =>
                {
                    if (!existentUser.Locations.Select(er => er.ID).Contains(l.ID))
                        existentUser.Locations.Add(locations.Single(x => x.ID == l.ID));
                });

                existentUser.Locations.ToList().ForEach(er =>
                {
                    if (!user.Locations.Select(r => r.ID).Contains(er.ID))
                        existentUser.Locations.Remove(er);
                });
            }
            else
            {
                existentUser.Locations.ToList().ForEach(er =>
                {
                    existentUser.Locations.Remove(er);
                });
            }

            _unitOfWork.Get<User>().Update(existentUser);

            await _unitOfWork.SaveAsync();

            return new SuccessResponse<UserDto>();
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            var existentUser = await _unitOfWork.Get<User>()
                .GetAsync(u => u.ID == id);

            _unitOfWork.Get<User>().Delete(existentUser);

            await _unitOfWork.SaveAsync(); 
            
            return new SuccessResponse<bool>(true);
        }

        public async Task<ServiceResponse<UserDto>> GetAsync(string login, int? locationId)
        {
            var user = await _unitOfWork.Get<User>()
                .GetAsync(u => u.Login.Equals(login), 
                    u => u.Locations,
                    u => u.Roles);

            if (user == null)
                return new ServiceResponse<UserDto>(ServiceResponseStatus.NotFound);

            if (!user.Locations.Any())
                return new SuccessResponse<UserDto>(user.ToDto());

            if (!locationId.HasValue || !user.Locations.Select(l => l.ID).Contains(locationId.Value))
                return new ServiceResponse<UserDto>(ServiceResponseStatus.NotFound);

            return new SuccessResponse<UserDto>(user.ToDto());
        }

        public async Task<ServiceResponse<UserDto>> GetAsync(string login)
        {
            var user = await _unitOfWork.Get<User>()
                .GetAsync(u => u.Login.Equals(login),
                    u => u.Roles,
                    u => u.Locations);

            return user == null
                ? new ServiceResponse<UserDto>(ServiceResponseStatus.NotFound)
                : new SuccessResponse<UserDto>(user.ToDto());
        }

        public async Task<ServiceResponse<UserDto>> GetAsync(int id)
        {
            var user = await _unitOfWork.Get<User>()
                .GetAsync(u => u.ID == id,
                    u => u.Roles,
                    u => u.Locations);

            return new SuccessResponse<UserDto>(user.ToDto());           
        }

        public async Task<ServiceResponse<UserDto>> GetAsync(string login, string password, int? locationId)
        {
            var hash = CryptoHelper.Hash(password);

            var user = await _unitOfWork.Get<User>()
                .GetAsync(u => !u.IsBlocked && 
                               u.Login.Equals(login) && 
                               u.Password.Equals(hash), 
                    u => u.Locations, 
                    u => u.Roles);

            if (user == null)
                return new ServiceResponse<UserDto>(ServiceResponseStatus.NotFound);

            if (!user.Locations.Any())
                return new SuccessResponse<UserDto>(user.ToDto());
            
            if (!locationId.HasValue || !user.Locations.Select(l => l.ID).Contains(locationId.Value))
                return new ServiceResponse<UserDto>(ServiceResponseStatus.NotFound);

            return new SuccessResponse<UserDto>(user.ToDto());
        }

        public async Task<ServiceResponse<UserDto>> GetAsync(string login, string password)
        {
            var hash = CryptoHelper.Hash(password);

            var user = await _unitOfWork.Get<User>()
                .GetAsync(u => !u.IsBlocked
                               && u.Login.Equals(login)
                               && u.Password.Equals(hash),
                    u => u.Roles,
                    u => u.Locations);

            return user == null
                ? new ServiceResponse<UserDto>(ServiceResponseStatus.NotFound)
                : new SuccessResponse<UserDto>(user.ToDto());
        }

        public async Task<ServiceResponse<bool>> ChangePasswordAsync(string login, string newPassword)
        {
            var user = await _unitOfWork.Get<User>()
                .GetAsync(u => u.Login.Equals(login));

            user.Password = CryptoHelper.Hash(newPassword);

            _unitOfWork.Get<User>().Update(user);

            await _unitOfWork.SaveAsync();  
            
            return new SuccessResponse<bool>(true);
        }

        public ServiceResponse<bool> Exists(string login, int userId = 0)
        {
            if (string.IsNullOrEmpty(login))
                return new SuccessResponse<bool>();

            var userWithSameLogin = _unitOfWork.Get<User>()
                .GetAll(u => u.Login.Equals(login) && 
                             u.ID != userId);

            return new SuccessResponse<bool>(userWithSameLogin.Any());
        }

        public ServiceResponse<UserDto> Get(string login)
        {
            var user = _unitOfWork.Get<User>()
                .Get(u => u.Login.Equals(login),
                    u => u.Roles,
                    u => u.Locations);

            return new SuccessResponse<UserDto>(user.ToDto());
        }
    }
}