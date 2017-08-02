using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class RecommendationModel
    {
        public int ID { get; set; }

        public int ParentID { get; set; }

        [Display(Name = "Значение")]
        public string ValueString { get; set; }
        
        [Display(Name = "Текст")]
        public string Comment { get; set; }
    }
}