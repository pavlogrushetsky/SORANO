using System;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class DeliveryItemModel : EntityBaseModel
    {
        [Display(Name = "Поставка")]
        public int DeliveryID { get; set; }

        [Display(Name = "Артикул")]
        public int ArticleID { get; set; }

        [Display(Name = "Количество, шт.")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public int Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Стоимость единицы товара должна быть больше 0")]
        public decimal UnitPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Общая стоимость позиции должна быть больше 0")]
        public decimal GrossPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        [Range(0.0, Double.MaxValue, ErrorMessage = "Сумма скидки должна быть больше или равна 0")]
        public decimal Discount { get; set; }

        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Стоимость с учётом скидки должна быть больше 0")]
        public decimal DiscountPrice { get; set; }

        public DeliveryModel Delivery { get; set; }

        public ArticleModel Article { get; set; }
    }
}