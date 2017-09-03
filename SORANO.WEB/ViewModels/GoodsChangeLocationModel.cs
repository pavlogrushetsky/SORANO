using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels
{
    public class GoodsChangeLocationModel
    {
        public int ArticleID { get; set; }

        [Display(Name = "Артикул")]
        public string ArticleName { get; set; }

        [Display(Name = "Количество, шт.")]
        public int Count { get; set; }

        [Display(Name = "Максимальное количество, шт.")]
        public int MaxCount { get; set; }

        [Display(Name = "Текущее место")]
        public string CurrentLocationName { get; set; }

        [Display(Name = "Место")]
        public int TargetLocationID { get; set; }

        [Display(Name = "Место")]
        public string TargetLocationName { get; set; }

        public int CurrentLocationID { get; set; }

        public string ReturnUrl { get; set; }
    }
}
