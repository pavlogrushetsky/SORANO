using System;

namespace SORANO.BLL.Dtos
{
    public class GoodsDto : BaseDto
    {
        public int DeliveryItemID { get; set; }

        public int? ClientID { get; set; }

        public decimal? SalePrice { get; set; }

        public DateTime? SaleDate { get; set; }

        public int? SoldBy { get; set; }

        public int SaleLocationID { get; set; }

        public DeliveryItemDto DeliveryItem { get; set; }

        public ClientDto Client { get; set; }

        public string SoldByUser { get; set; }

        public LocationDto SaleLocation { get; set; }
    }
}