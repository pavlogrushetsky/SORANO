using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SORANO.WEB.Models
{
    public class UserModel
    {
        [BindNever]
        public int ID { get; set; }

        [Required(ErrorMessage = "Необходимо указать имя пользователя")]
        [MaxLength(100, ErrorMessage = "Длина имени пользователя не должна превышать 100 символов")]
        [MinLength(5, ErrorMessage = "Длина имени пользователя должна содержать не менее 5 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Необходимо указать Email пользователя")]
        [MaxLength(200, ErrorMessage = "Длина Email не должна превышать 200 символов")]
        [MinLength(5, ErrorMessage = "Длина Email должна содержать не менее 5 символов")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Необходимо указать пароль пользователя")]
        public string Password { get; set; }

        public bool IsBlocked { get; set; }

        public List<string> Roles { get; set; }
    }
}