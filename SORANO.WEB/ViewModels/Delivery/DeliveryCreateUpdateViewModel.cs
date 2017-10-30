using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;
using SORANO.WEB.ViewModels.DeliveryItem;

namespace SORANO.WEB.ViewModels.Delivery
{
    public class DeliveryCreateUpdateViewModel : BaseCreateUpdateViewModel
    {
        [Display(Name = "Номер накладной *")]
        public string BillNumber { get; set; }

        public string TotalGrossPrice { get; set; }

        public string TotalDiscount { get; set; }

        public string TotalDiscountPrice { get; set; }

        public string SelectedCurrency { get; set; } = "₴";

        [Display(Name = "Поставщик *")]
        public int SupplierID { get; set; }

        public string SupplierName { get; set; }

        [Display(Name = "Место поставки *")]
        public int LocationID { get; set; }

        public string LocationName { get; set; }

        [Display(Name = "Дата поставки *")]
        public string DeliveryDate { get; set; }

        [Display(Name = "Дата оплаты")]
        public string PaymentDate { get; set; }

        public string DollarRate { get; set; }

        public string EuroRate { get; set; }

        public bool Status { get; set; }

        public int DeliveryItemsCount { get; set; }

        public IEnumerable<DeliveryItemViewModel> DeliveryItems { get; set; } = new List<DeliveryItemViewModel>();
    }
}