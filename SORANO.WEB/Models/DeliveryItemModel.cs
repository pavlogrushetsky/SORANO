namespace SORANO.WEB.Models
{
    public class DeliveryItemModel : EntityBaseModel
    {
        public int DeliveryID { get; set; }

        public int ArticleID { get; set; }

        public int Quantity { get; set; }

        public string UnitPrice { get; set; }

        public string GrossPrice { get; set; }

        public string Discount { get; set; }

        public string DiscountPrice { get; set; }

        public DeliveryModel Delivery { get; set; }

        public ArticleModel Article { get; set; }
    }
}