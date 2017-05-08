using System.ComponentModel.DataAnnotations;
using SORANO.WEB.Infrastructure.ValidationAttributes;

namespace SORANO.WEB.Models.Account
{
    public class ChangePasswordModel
    {
        public string Description { get; set; }

        public string Login { get; set; }

        [Required(ErrorMessage = "Необходимо указать существующий пароль")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Необходимо указать новый пароль")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Необходимо повторно указать новый пароль")]
        [DataType(DataType.Password)]
        [RequireEqual("NewPassword", ErrorMessage = "Необходимо, чтобы значения полей \"Новый пароль\" совпадали")]
        public string RepeatPassword { get; set; }

        public string ReturnUrl { get; set; }
    }
}