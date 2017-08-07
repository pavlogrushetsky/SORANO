using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class DeliveryItemModel : EntityBaseModel
    {
        [Display(Name = "Поставка")]
        public int DeliveryID { get; set; }

        [Display(Name = "Артикул")]
        public string ArticleID { get; set; }

        [Display(Name = "Количество, шт.")]
        public int Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        public decimal UnitPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        public decimal GrossPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        public decimal Discount { get; set; }

        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        public decimal DiscountPrice { get; set; }

        public DeliveryModel Delivery { get; set; }

        public ArticleModel Article { get; set; }
    }
}