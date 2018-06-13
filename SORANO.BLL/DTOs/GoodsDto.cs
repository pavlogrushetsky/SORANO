using System;
using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class GoodsDto : BaseDto
    {
        // ReSharper disable once InconsistentNaming
        public List<int> IDs { get; set; }

        public int DeliveryItemID { get; set; }

        public int? ClientID { get; set; }

        public decimal? SalePrice { get; set; }

        public decimal? Price { get; set; }

        public decimal? RecommendedPrice { get; set; }

        public DateTime? SaleDate { get; set; }

        public int? SoldBy { get; set; }

        public int? SaleLocationID { get; set; }

        public DeliveryItemDto DeliveryItem { get; set; }

        public ClientDto Client { get; set; }

        public string SoldByUser { get; set; }

        public LocationDto SaleLocation { get; set; }

        public IEnumerable<StorageDto> Storages { get; set; }

        public int Quantity { get; set; }

        public int? SaleID { get; set; }

        public SaleDto Sale { get; set; }

        public bool IsSold { get; set; }
    }
}