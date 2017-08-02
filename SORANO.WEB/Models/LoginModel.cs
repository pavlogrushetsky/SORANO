using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class LoginModel
    {
        [Display(Name = "Логин:")]
        public string Login { get; set; }

        [Display(Name = "Пароль:")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}