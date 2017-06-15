using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models.Location
{
    public class LocationModel : EntityBaseModel
    {
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Необходимо указать название места")]
        [MaxLength(200, ErrorMessage = "Длина названия не должна превышать 200 символов")]
        [MinLength(5, ErrorMessage = "Длина названия должна содержать не менее 5 символов")]
        public string Name { get; set; }

        [Display(Name = "Коментарий")]
        [MaxLength(1000, ErrorMessage = "Длина коментария не должна превышать 1000 символов")]
        public string Comment { get; set; }

        [Display(Name = "Тип")]
        public string Type { get; set; }
    }
}
