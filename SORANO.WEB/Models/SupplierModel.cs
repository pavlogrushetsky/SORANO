using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class SupplierModel : EntityBaseModel
    {
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Необходимо указать название поставщика")]
        [MaxLength(200, ErrorMessage = "Длина названия не должна превышать 200 символов")]
        [MinLength(5, ErrorMessage = "Длина названия должна содержать не менее 5 символов")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        [MaxLength(1000, ErrorMessage = "Длина описания не должна превышать 1000 символов")]
        public string Description { get; set; }
    }
}
