using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class ChangePasswordModel : IValidatableObject
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
        public string RepeatPassword { get; set; }

        public string ReturnUrl { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var errors = new List<ValidationResult>();

            if (!NewPassword.Equals(RepeatPassword))
            {
                errors.Add(new ValidationResult("Необходимо повторно ввести новый пароль", new List<string>{ "RepeatPassword" }));
            }

            return errors;
        }
    }
}