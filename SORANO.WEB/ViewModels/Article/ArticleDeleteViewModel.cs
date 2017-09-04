using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.Article
{
    public class ArticleDeleteViewModel
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

        public bool CanBeDeleted { get; set; }
    }
}