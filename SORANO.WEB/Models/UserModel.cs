using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SORANO.WEB.Models
{
    public class UserModel
    {
        [BindNever]
        public int ID { get; set; }

        [MaxLength(100, ErrorMessage = "Длина описания пользователя не должна превышать 200 символов")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Необходимо указать логин пользователя")]
        [MaxLength(200, ErrorMessage = "Длина логина не должна превышать 200 символов")]
        [MinLength(5, ErrorMessage = "Длина логина должна содержать не менее 5 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Необходимо указать пароль пользователя")]
        public string Password { get; set; }

        public bool IsBlocked { get; set; }

        public List<string> Roles { get; set; }
    }
}