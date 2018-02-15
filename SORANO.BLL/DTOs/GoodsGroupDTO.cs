namespace SORANO.BLL.Dtos
{
    public class GoodsGroupDto
    {
        public int Count { get; set; }

        public int LocationId { get; set; }

        public string LocationName { get; set; }

        public int DeliveryId { get; set; }

        public string BillNumber { get; set; }

        public decimal DeliveryPrice { get; set; }

        public decimal? DollarRate { get; set; }

        public decimal? EuroRate { get; set; }
    }
}