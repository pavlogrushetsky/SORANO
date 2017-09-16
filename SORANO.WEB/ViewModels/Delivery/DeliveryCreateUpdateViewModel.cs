using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.Delivery
{
    public class DeliveryCreateUpdateViewModel : BaseCreateUpdateViewModel
    {
        public bool Status { get; set; }

        public int DeliveryItemsCount { get; set; }
    }
}