using System.Collections.Generic;
using System.Linq;

namespace SORANO.BLL.Dtos.ReportDtos
{
    public class SaleDto
    {
        public decimal Cash { get; set; }
        public decimal Cashless { get; set; }
    }

    public class LocationSalesDto
    {
        public Dictionary<string, SaleDto> LocationSales { get; set; }
        public decimal Total => CashTotal + CashlessTotal;
        public decimal CashTotal => LocationSales.Values.Sum(v => v.Cash);
        public decimal CashlessTotal => LocationSales.Values.Sum(v => v.Cashless);
    }
}