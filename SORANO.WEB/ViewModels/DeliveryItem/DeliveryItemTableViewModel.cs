using System.Collections.Generic;

namespace SORANO.WEB.ViewModels.DeliveryItem
{
    public class DeliveryItemTableViewModel
    {
        public DeliveryItemTableMode Mode { get; set; }

        public int DeliveryId { get; set; }

        public IList<DeliveryItemViewModel> Items { get; set; }

        public DeliveryItemsSummaryViewModel Summary { get; set; }
    }
}