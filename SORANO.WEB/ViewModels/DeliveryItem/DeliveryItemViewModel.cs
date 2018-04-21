using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.DeliveryItem
{
    public class DeliveryItemViewModel : BaseCreateUpdateViewModel
    {
        [Display(Name = "Артикул *")]
        public int ArticleID { get; set; }

        public string ArticleName { get; set; }

        [Display(Name = "Количество, шт. *")]
        public int Quantity { get; set; }

        [Display(Name = "Стоимость ед. товара *")]
        public string UnitPrice { get; set; }

        [Display(Name = "Общая стоимость")]
        public string GrossPrice { get; set; }

        [Display(Name = "Скидка")]
        public string Discount { get; set; }

        [Display(Name = "Стоимость с учётом скидки")]
        public string DiscountedPrice { get; set; }

        public int DeliveryID { get; set; }

        [Display(Name = "Поставка")]
        public string DeliveryBillNumber { get; set; }

        [Display(Name = "Дата поставки")]
        public string DeliveryDate { get; set; }

        public string ReturnPath { get; set; }

        public string Currency { get; set; }

        public bool CanBeDeleted { get; set; }

        public int Number { get; set; }
    }
}