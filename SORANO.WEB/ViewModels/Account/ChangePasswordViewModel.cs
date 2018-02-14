using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.Account
{
    public class ChangePasswordViewModel
    {
        public string Login { get; set; }

        [Display(Name = "Существующий пароль")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "Новый пароль")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Новый пароль")]
        [DataType(DataType.Password)]
        public string RepeatPassword { get; set; }

        public string ReturnUrl { get; set; }
    }
}
