using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.ArticleType
{
    public class ArticleTypeBriefViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Статус")]
        public bool IsDeleted { get; set; }

        public bool HasChildTypes { get; set; }

        [Display(Name = "Вложенные типы")]
        public int ChildTypesCount { get; set; }

        public bool HasArticles { get; set; }

        [Display(Name = "Артикулы")]
        public int ArticlesCount { get; set; }

        public bool HasRecommendations { get; set; }

        [Display(Name = "Рекомендации")]
        public int RecommendationsCount { get; set; }

        [Display(Name = "Вложения")]
        public int AttachmentsCount { get; set; }
        
        public bool CanBeDeleted { get; set; }
    }
}