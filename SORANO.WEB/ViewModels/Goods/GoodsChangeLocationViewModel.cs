using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Attachment;

namespace SORANO.WEB.ViewModels.Goods
{
    public class GoodsChangeLocationViewModel
    {
        public string Ids { get; set; }

        [Display(Name = "Артикул")]
        public string ArticleName { get; set; }

        [Display(Name = "Описание")]
        public string ArticleDescription { get; set; }

        [Display(Name = "Тип артикула")]
        public string ArticleTypeName { get; set; }

        [Display(Name = "Количество, шт.")]
        public int Count { get; set; }

        [Display(Name = "Максимальное количество, шт.")]
        public int MaxCount { get; set; }

        [Display(Name = "Текущий склад")]
        public string CurrentLocationName { get; set; }

        [Display(Name = "Текущий склад")]
        public int CurrentLocationID { get; set; }

        [Display(Name = "Склад")]
        public int TargetLocationID { get; set; }

        [Display(Name = "Склад")]
        public string TargetLocationName { get; set; }

        public MainPictureViewModel MainPicture { get; set; }

        public bool HasMainPicture => !string.IsNullOrWhiteSpace(MainPicture.FullPath);
    }
}
