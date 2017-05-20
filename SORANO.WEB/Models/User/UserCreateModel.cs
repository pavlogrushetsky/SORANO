using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SORANO.WEB.Infrastructure.ValidationAttributes;
using SORANO.WEB.Models.Role;

namespace SORANO.WEB.Models.User
{
    public class UserCreateModel
    {
        [MaxLength(1000, ErrorMessage = "Длина описания пользователя не должна превышать 1000 символов")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Необходимо указать логин пользователя")]
        [MaxLength(100, ErrorMessage = "Длина логина не должна превышать 100 символов")]
        [MinLength(5, ErrorMessage = "Длина логина должна содержать не менее 5 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Необходимо указать пароль пользователя")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [RequireNonEmpty(ErrorMessage = "Пользователю необходимо назначить хотя бы одну роль")]
        public IList<RoleModel> Roles { get; set; } = new List<RoleModel>();
    }
}