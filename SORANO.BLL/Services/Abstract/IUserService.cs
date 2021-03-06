﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IUserService
    {
        Task<ServiceResponse<UserDto>> GetAsync(string login, string password);

        Task<ServiceResponse<UserDto>> GetAsync(string login, string password, int? locationID);

        Task<ServiceResponse<UserDto>> GetAsync(string login);

        Task<ServiceResponse<UserDto>> GetAsync(string login, int? locationId);

        Task<ServiceResponse<UserDto>> GetAsync(int id);

        ServiceResponse<UserDto> Get(string login);

        ServiceResponse<IEnumerable<UserDto>> GetAll();

        Task<ServiceResponse<UserDto>> CreateAsync(UserDto user);

        Task<ServiceResponse<bool>> ChangePasswordAsync(string login, string newPassword);

        Task<ServiceResponse<bool>> DeleteAsync(int id);

        Task<ServiceResponse<UserDto>> UpdateAsync(UserDto user);

        ServiceResponse<bool> Exists(string login, int userId = 0);
    }
}