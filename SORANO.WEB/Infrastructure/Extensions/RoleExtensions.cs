using SORANO.CORE.AccountEntities;
using SORANO.WEB.Models.Role;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class RoleExtensions
    {
        public static RoleModel ToModel(this Role role)
        {
            return new RoleModel
            {
                ID = role.ID,
                Name = role.Name,
                Description = role.Description
            };
        }
    }
}
