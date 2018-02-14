using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;
using SORANO.WEB.ViewModels.DeliveryItem;

namespace SORANO.WEB.ViewModels.Delivery
{
    public class DeliveryDetailsViewModel : BaseDetailsViewModel
    {
        [Display(Name = "Валюта")]
        public string Currency { get; set; }

        [Display(Name = "Номер накладной")]
        public string BillNumber { get; set; }

        [Display(Name = "Дата поставки")]
        public string DeliveryDate { get; set; }

        [Display(Name = "Дата оплаты")]
        public string PaymentDate { get; set; }

        [Display(Name = "Курс доллара")]
        public string DollarRate { get; set; }

        [Display(Name = "Курс евро")]
        public string EuroRate { get; set; }

        public int SupplierID { get; set; }

        [Display(Name = "Поставщик")]
        public string SupplierName { get; set; }

        public int LocationID { get; set; }

        [Display(Name = "Место поставки")]
        public string LocationName { get; set; }

        [Display(Name = "Статус")]
        public bool IsSubmitted { get; set; }

        [Display(Name = "Общая сумма")]
        public string TotalGrossPrice { get; set; }

        [Display(Name = "Сумма скидки")]
        public string TotalDiscount { get; set; }

        [Display(Name = "С учётом скидки")]
        public string TotalDiscountedPrice { get; set; }

        [Display(Name = "Позиции поставки")]
        public DeliveryItemTableViewModel Table { get; set; }
    }
}
