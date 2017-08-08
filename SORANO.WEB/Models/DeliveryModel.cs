using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class DeliveryModel : EntityBaseModel
    {
        [Display(Name = "Номер накладной")]
        public string BillNumber { get; set; }

        [Display(Name = "Дата поставки")]
        public string DeliveryDate { get; set; }

        [Display(Name = "Дата оплаты")]
        public string PaymentDate { get; set; }

        [Display(Name = "Курс доллара США")]
        public decimal? DollarRate { get; set; }

        [Display(Name = "Курс евро")]
        public decimal? EuroRate { get; set; }

        [Display(Name = "Общая сумма")]
        public decimal TotalGrossPrice { get; set; }

        [Display(Name = "Размер скидки")]
        public decimal TotalDiscount { get; set; }

        [Display(Name = "Общая сумма с учётом скидки")]
        public decimal TotalDiscountPrice { get; set; }

        [Display(Name = "Статус")]
        public bool Status { get; set; }

        [Display(Name = "Поставщик")]
        public SupplierModel Supplier { get; set; }

        [Display(Name = "Поставщик")]
        public string SupplierID { get; set; }

        [Display(Name = "Место поставки")]
        public string LocationID { get; set; }

        [Display(Name = "Место поставки")]
        public LocationModel Location { get; set; }

        [Display(Name = "Единиц поставки")]
        public int DeliveryItemsCount => DeliveryItems.Count;

        public List<DeliveryItemModel> DeliveryItems { get; set; } = new List<DeliveryItemModel>();

        public int CurrentItemNumber { get; set; }

        public int SelectedCurrency { get; set; }

        public bool CanBeUpdated { get; set; }
    }
}