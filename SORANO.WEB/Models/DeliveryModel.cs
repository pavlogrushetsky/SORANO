using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class DeliveryModel : EntityBaseModel
    {
        [Display(Name = "Номер накладной")]
        [Required(ErrorMessage = "Необходимо указать номер накладной")]
        [MaxLength(100, ErrorMessage = "Длина номера накладной не должна превышать 100 символов")]
        public string BillNumber { get; set; }

        [Display(Name = "Дата поставки")]
        [Required(ErrorMessage = "Необходимо указать дату поставки")]
        [DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }

        [Display(Name = "Дата оплаты")]
        [DataType(DataType.Date)]
        public DateTime? PaymentDate { get; set; }

        [Display(Name = "Курс доллара США")]
        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        public decimal? DollarRate { get; set; }

        [Display(Name = "Курс евро")]
        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        public decimal? EuroRate { get; set; }

        [Display(Name = "Общая сумма")]
        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        public decimal TotalGrossPrice { get; set; }

        [Display(Name = "Размер скидки")]
        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        public decimal TotalDiscount { get; set; }

        [Display(Name = "Общая сумма с учётом скидки")]
        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        public decimal TotalDiscountPrice { get; set; }

        [Display(Name = "Статус")]
        public bool Status { get; set; }

        [Display(Name = "Поставщик")]
        public SupplierModel Supplier { get; set; }

        [Display(Name = "Поставщик")]
        [Required(ErrorMessage = "Необходимо указать поставщика")]
        public string SupplierID { get; set; }

        [Display(Name = "Место поставки")]
        [Required(ErrorMessage = "Необходимо указать место поставки")]
        public string LocationID { get; set; }

        [Display(Name = "Место поставки")]
        public LocationModel Location { get; set; }

        [Display(Name = "Единиц поставки")]
        public int DeliveryItemsCount => DeliveryItems.Count;

        public List<DeliveryItemModel> DeliveryItems { get; set; } = new List<DeliveryItemModel>();

        public int CurrentItemNumber { get; set; }

        public int SelectedCurrency { get; set; }
    }
}