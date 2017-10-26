using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.Delivery
{
    public class DeliveryItemViewModel : BaseCreateUpdateViewModel
    {
        public int Number { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public string UnitPrice { get; set; }

        public string GrossPrice { get; set; }

        public string Discount { get; set; }

        public string DiscountPrice { get; set; }
    }
}