using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.Goods
{
    public class GoodsRecommendationsViewModel : BaseCreateUpdateViewModel
    {
        public string Ids { get; set; }

        [Display(Name = "Артикул")]
        public string ArticleName { get; set; }

        [Display(Name = "Описание")]
        public string ArticleDescription { get; set; }

        [Display(Name = "Тип артикула")]
        public string ArticleTypeName { get; set; }

        [Display(Name = "Склад")]
        public string LocationName { get; set; }

        [Display(Name = "Цена поставки")]
        public string DeliveryPrice { get; set; }

        public string Currency { get; set; }

        public int Quantity { get; set; }

        public bool HasMainPicture => !string.IsNullOrWhiteSpace(MainPicture.FullPath);
    }
}