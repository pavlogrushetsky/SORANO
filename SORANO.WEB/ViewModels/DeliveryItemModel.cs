using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels
{
    public class DeliveryItemModel : EntityBaseModel
    {
        [Display(Name = "Поставка")]
        public int DeliveryID { get; set; }

        [Display(Name = "Артикул")]
        public string ArticleID { get; set; }

        [Display(Name = "Количество, шт.")]
        public int Quantity { get; set; }

        public string UnitPrice { get; set; }

        public string GrossPrice { get; set; }

        public string Discount { get; set; }

        public string DiscountPrice { get; set; }

        public DeliveryModel Delivery { get; set; }
    }
}