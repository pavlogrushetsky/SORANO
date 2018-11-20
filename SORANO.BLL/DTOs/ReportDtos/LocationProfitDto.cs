using System.Collections.Generic;

namespace SORANO.BLL.Dtos.ReportDtos
{
    public class LocationProfitDto
    {
        public List<Dictionary<string, decimal>> LocationProfit { get; set; }
        public decimal Total { get; set; }
    }
}