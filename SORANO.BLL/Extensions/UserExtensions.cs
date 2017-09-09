using System.Linq;
using SORANO.BLL.Dtos;
using SORANO.CORE.AccountEntities;

namespace SORANO.BLL.Extensions
{
    internal static class UserExtensions
    {
        public static UserDto ToDto(this User model)
        {
            return new UserDto
            {
                ID = model.ID,
                Login = model.Login,
                Description = model.Description,
                IsBlocked = model.IsBlocked,
                Roles = model.Roles.Select(r => r.ToDto())
            };
        }

        public static User ToEntity(this UserDto dto)
        {
            return new User
            {
                ID = dto.ID,
                Login = dto.Login,
                Description = dto.Description,
                Password = dto.Password,
                Roles = dto.Roles.Select(r => r.ToEntity()).ToList()
            };
        }
    }
}