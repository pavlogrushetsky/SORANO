using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class DeliveryModel : EntityBaseModel
    {
        [Display(Name = "Номер накладной")]
        [Required(ErrorMessage = "Необходимо указать номер накладной")]
        [MaxLength(200, ErrorMessage = "Длина номера накладной не должна превышать 200 символов")]
        public string BillNumber { get; set; }

        [Display(Name = "Дата поставки")]
        [Required(ErrorMessage = "Необходимо указать дату поставки")]
        public string DeliveryDate { get; set; }

        [Display(Name = "Дата оплаты")]
        public string PaymentDate { get; set; }

        [Display(Name = "Курс доллара США")]
        public string DollarRate { get; set; }

        [Display(Name = "Курс евро")]
        public string EuroRate { get; set; }

        [Display(Name = "Общая сумма")]
        public string TotalGrossPrice { get; set; }

        [Display(Name = "Размер скидки")]
        public string TotalDiscount { get; set; }

        [Display(Name = "Общая сумма с учётом скидки")]
        public string TotalDiscountPrice { get; set; }

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
    }
}