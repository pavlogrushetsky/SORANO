using System.Collections.Generic;

namespace SORANO.BLL.Dtos.ReportDtos
{
    public class LocationSalesDto
    {
        public Dictionary<string, decimal> LocationSales { get; set; }
        public decimal Total { get; set; }
    }
}