using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;
using SORANO.WEB.ViewModels.Delivery;

namespace SORANO.WEB.ViewModels.Supplier
{
    public class SupplierDetailsViewModel : BaseDetailsViewModel
    {
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Поставки")]
        public DeliveryIndexViewModel Deliveries { get; set; }
    }
}
