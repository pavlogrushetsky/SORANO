using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models.Recommendation
{
    public class RecommendationModel
    {
        public int ID { get; set; }

        [Required]
        public int ParentID { get; set; }

        [Display(Name = "Значение")]
        public string ValueString { get; set; }

        [Required]
        [Display(Name = "Текст")]
        [MaxLength(500, ErrorMessage = "Длина текста не должна превышать 1000 символов")]
        [MinLength(5, ErrorMessage = "Длина текста должна содержать не менее 5 символов")]
        public string Comment { get; set; }
    }
}