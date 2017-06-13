using SORANO.WEB.Models.Recommendation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models.Supplier
{
    public class SupplierModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Название")]
        [Required(ErrorMessage = "Необходимо указать название поставщика")]
        [MaxLength(500, ErrorMessage = "Длина названия не должна превышать 200 символов")]
        [MinLength(5, ErrorMessage = "Длина названия должна содержать не менее 5 символов")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        [MaxLength(1000, ErrorMessage = "Длина описания не должна превышать 1000 символов")]
        public string Description { get; set; }

        [Display(Name = "Рекомендации")]
        public List<RecommendationModel> Recommendations { get; set; } = new List<RecommendationModel>();
    }
}
