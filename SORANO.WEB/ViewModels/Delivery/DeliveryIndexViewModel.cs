using System.Collections.Generic;

namespace SORANO.WEB.ViewModels.Delivery
{
    public class DeliveryIndexViewModel
    {
        public DeliveryTableMode Mode { get; set; }

        public IList<DeliveryViewModel> Items { get; set; }

        public bool ShowLocation { get; set; }
    }
}
