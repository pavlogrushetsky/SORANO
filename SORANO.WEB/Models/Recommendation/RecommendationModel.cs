using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models.Recommendation
{
    public class RecommendationModel
    {
        public int ID { get; set; }

        [Required]
        public int ParentID { get; set; }

        [Display(Name = "Значение")]
        [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})?$", ErrorMessage = "Значение рекомендации должно быть в формате x.xx")]
        public string ValueString { get; set; }
        
        [Display(Name = "Текст")]
        [Required(ErrorMessage = "Необходимо указать текст рекомендации")]
        [MaxLength(500, ErrorMessage = "Длина текста рекомендации не должна превышать 1000 символов")]
        [MinLength(5, ErrorMessage = "Длина текста рекомендации должна содержать не менее 5 символов")]
        public string Comment { get; set; }
    }
}