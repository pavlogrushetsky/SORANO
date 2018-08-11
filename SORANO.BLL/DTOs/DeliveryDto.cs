using System;
using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class DeliveryDto : BaseDto
    {
        public int SupplierID { get; set; }

        public SupplierDto Supplier { get; set; }

        public int LocationID { get; set; }

        public LocationDto Location { get; set; }

        public string BillNumber { get; set; }

        public DateTime DeliveryDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        public decimal? DollarRate { get; set; }

        public decimal? EuroRate { get; set; }

        public decimal TotalGrossPrice { get; set; }

        public decimal TotalDiscount { get; set; }

        public decimal TotalDiscountedPrice { get; set; }

        public bool IsSubmitted { get; set; }

        public int ItemsCount { get; set; }

        public IEnumerable<DeliveryItemDto> Items { get; set; }
    }
}