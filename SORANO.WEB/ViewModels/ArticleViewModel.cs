using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels
{
    public class ArticleViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Производитель")]
        public string Producer { get; set; }

        [Display(Name = "Код")]
        public string Code { get; set; }

        [Display(Name = "Штрих-код")]
        public string Barcode { get; set; }

        public string ReturnPath { get; set; }

        public List<RecommendationViewModel> Recommendations { get; set; }
    }
}