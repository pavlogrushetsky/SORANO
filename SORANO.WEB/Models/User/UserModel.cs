using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SORANO.WEB.Infrastructure.ValidationAttributes;
using SORANO.WEB.Models.Role;

namespace SORANO.WEB.Models.User
{
    public class UserModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Необходимо указать логин пользователя")]
        [MaxLength(100, ErrorMessage = "Длина логина не должна превышать 100 символов")]
        [MinLength(5, ErrorMessage = "Длина логина должна содержать не менее 5 символов")]
        public string Login { get; set; }

        [Display(Name = "Описание")]
        [MaxLength(1000, ErrorMessage = "Длина описания пользователя не должна превышать 1000 символов")]
        public string Description { get; set; }       

        [RequiredIfCreate(ErrorMessage = "Необходимо указать пароль пользователя")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [RequireEqual("NewPassword", ErrorMessage = "Необходимо, чтобы значения полей \"Новый пароль\" совпадали")]
        public string RepeatPassword { get; set; }

        [Display(Name = "Роли")]
        [RequireNonEmpty(ErrorMessage = "Пользователю необходимо назначить хотя бы одну роль")]
        public IList<RoleModel> Roles { get; set; } = new List<RoleModel>();

        [Display(Name = "Продажи")]
        public List<UserSaleModel> Sales { get; set; } = new List<UserSaleModel>();

        [Display(Name = "Действия")]
        public List<UserActivityModel> Activities { get; set; } = new List<UserActivityModel>();

        [Display(Name = "Статус")]
        public bool IsBlocked { get; set; }

        public bool CanBeDeleted { get; set; }

        public bool CanBeModified { get; set; }

        public bool CanBeBlocked { get; set; }
    }
}