using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SORANO.WEB.Infrastructure.ValidationAttributes;

namespace SORANO.WEB.Models
{
    public class UserModel
    {
        public int ID { get; set; }

        [MaxLength(1000, ErrorMessage = "Длина описания пользователя не должна превышать 1000 символов")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Необходимо указать логин пользователя")]
        [MaxLength(100, ErrorMessage = "Длина логина не должна превышать 100 символов")]
        [MinLength(5, ErrorMessage = "Длина логина должна содержать не менее 5 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Необходимо указать пароль пользователя")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsBlocked { get; set; }

        [RequireNonEmpty(ErrorMessage = "Пользователю необходимо назначить хотя бы одну роль")]
        public IEnumerable<string> Roles { get; set; }

        public List<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "developer", Text = "Разработчик" },
            new SelectListItem { Value = "administrator", Text = "Администратор" },
            new SelectListItem { Value = "editor", Text = "Редактор" },
            new SelectListItem { Value = "manager", Text = "Менеджер" },
            new SelectListItem { Value = "user", Text = "Пользователь" },
        };
    }
}