using SORANO.BLL.Dtos;
using SORANO.CORE.AccountEntities;

namespace SORANO.BLL.Extensions
{
    internal static class RoleExtensions
    {
        public static RoleDto ToDto(this Role model)
        {
            return new RoleDto
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description
            };
        }

        public static Role ToEntity(this RoleDto dto)
        {
            return new Role
            {
                ID = dto.ID,
                Name = dto.Name,
                Description = dto.Description
            };
        }
    }
}