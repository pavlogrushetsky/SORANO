using System.Collections.Generic;

namespace SORANO.WEB.ViewModels.DeliveryItem
{
    public class DeliveryItemTableViewModel
    {
        public DeliveryItemTableMode Mode { get; set; }

        public IList<DeliveryItemViewModel> Items { get; set; }
    }
}