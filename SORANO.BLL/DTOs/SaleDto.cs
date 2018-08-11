using System;
using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class SaleDto : BaseDto
    {
        public int? ClientID { get; set; }

        public int LocationID { get; set; }

        public int UserID { get; set; }

        public bool IsSubmitted { get; set; }

        public decimal? TotalPrice { get; set; }

        public decimal? DollarRate { get; set; }

        public decimal? EuroRate { get; set; }

        public bool IsCachless { get; set; }

        public bool IsWriteOff { get; set; }

        public DateTime? Date { get; set; }

        public ClientDto Client { get; set; }

        public LocationDto Location { get; set; }

        public UserDto User { get; set; }

        public int GoodsCount { get; set; }

        public IEnumerable<GoodsDto> Goods { get; set; } = new HashSet<GoodsDto>();
    }
}