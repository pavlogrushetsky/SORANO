using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.Recommendation
{
    public class RecommendationViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Значение")]
        public string Value { get; set; }

        [Display(Name = "Коментарий")]
        public string Comment { get; set; }
    }
}