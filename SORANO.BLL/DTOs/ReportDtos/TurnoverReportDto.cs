using System;
using System.Collections.Generic;

namespace SORANO.BLL.Dtos.ReportDtos
{
    public class TurnoverReportDto
    {
        public Dictionary<DateTime, LocationDeliveriesDto> Deliveries { get; set; }
        public Dictionary<DateTime, LocationSalesDto> Sales { get; set; }
        public Dictionary<DateTime, LocationProfitDto> Profit { get; set; }
    }
}