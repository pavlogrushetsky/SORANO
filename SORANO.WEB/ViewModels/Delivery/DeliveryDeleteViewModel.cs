using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.Delivery
{
    public class DeliveryDeleteViewModel
    {
        public int ID { get; set; }

        public bool CanBeDeleted { get; set; }

        [Display(Name = "Номер накладной")]
        public string BillNumber { get; set; }

        [Display(Name = "Поставщик")]
        public string SupplierName { get; set; }

        [Display(Name = "Место поставки")]
        public string LocationName { get; set; }
    }
}
