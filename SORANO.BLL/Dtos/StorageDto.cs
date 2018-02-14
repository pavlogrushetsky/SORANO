using System;

namespace SORANO.BLL.Dtos
{
    public class StorageDto
    {
        public int ID { get; set; }

        public int GoodsID { get; set; }

        public int LocationID { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public GoodsDto Goods { get; set; }

        public LocationDto Location { get; set; }
    }
}