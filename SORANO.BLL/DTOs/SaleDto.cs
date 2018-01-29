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

        public DateTime? Date { get; set; }

        public ClientDto Client { get; set; }

        public LocationDto Location { get; set; }

        public UserDto User { get; set; }

        public IEnumerable<SaleItemDto> Items { get; set; } = new HashSet<SaleItemDto>();
    }
}