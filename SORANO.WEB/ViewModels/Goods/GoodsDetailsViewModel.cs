using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;
using SORANO.WEB.ViewModels.Recommendation;

namespace SORANO.WEB.ViewModels.Goods
{
    public class GoodsDetailsViewModel : BaseDetailsViewModel
    {
        public int ArticleID { get; set; }

        [Display(Name = "Артикул")]
        public string ArticleName { get; set; }

        [Display(Name = "Описание")]
        public string ArticleDescription { get; set; }

        public int ArticleTypeID { get; set; }

        [Display(Name = "Тип артикула")]
        public string ArticleTypeName { get; set; }

        public int LocationID { get; set; }

        [Display(Name = "Склад")]
        public string LocationName { get; set; }

        public int DeliveryID { get; set; }

        [Display(Name = "Поставка")]
        public string DeliveryBillNumber { get; set; }

        [Display(Name = "Статус")]
        public bool IsSold { get; set; }

        [Display(Name = "Цена поставки")]
        public string DeliveryPrice { get; set; }

        [Display(Name = "Дата поставки")]
        public string DeliveryDate { get; set; }

        [Display(Name = "Дата продажи")]
        public string SaleDate { get; set; }

        [Display(Name = "Цена продажи")]
        public string SalePrice { get; set; }

        [Display(Name = "Продавец")]
        public string SoldBy { get; set; }

        public string Currency { get; set; }

        [Display(Name = "Рекомендации позиции поставки")]
        public IList<RecommendationViewModel> DeliveryItemRecommendations { get; set; }

        [Display(Name = "Рекомендации поставки")]
        public IList<RecommendationViewModel> DeliveryRecommendations { get; set; }
    }
}