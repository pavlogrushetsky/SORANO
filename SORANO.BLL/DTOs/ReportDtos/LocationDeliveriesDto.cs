using System.Collections.Generic;

namespace SORANO.BLL.Dtos.ReportDtos
{
    public class LocationDeliveriesDto
    {
        public Dictionary<string, decimal> LocationDeliveries { get; set; }
        public decimal Total { get; set; }
    }
}