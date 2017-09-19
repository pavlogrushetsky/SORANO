using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.ArticleType
{
    public class ArticleTypeDeleteViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        public bool CanBeDeleted { get; set; }
    }
}