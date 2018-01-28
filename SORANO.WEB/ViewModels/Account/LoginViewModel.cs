using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.Account
{
    public class LoginViewModel
    {
        [Display(Name = "Логин *")]
        public string Login { get; set; }

        [Display(Name = "Пароль *")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }

        [Display(Name = "Магазин")]
        public int? LocationID { get; set; }

        public string LocationName { get; set; }

        public string ReturnUrl { get; set; }
    }
}
