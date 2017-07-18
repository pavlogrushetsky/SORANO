using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Необходимо указать логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Необходимо указать пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}