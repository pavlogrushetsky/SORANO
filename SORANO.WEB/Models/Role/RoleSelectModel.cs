using System.Collections.Generic;
using SORANO.WEB.Models.User;

namespace SORANO.WEB.Models.Role
{
    public class RoleSelectModel
    {
        public List<RoleModel> Roles { get; set; }

        public UserModel User { get; set; }

        public string ReturnUrl { get; set; }
    }
}