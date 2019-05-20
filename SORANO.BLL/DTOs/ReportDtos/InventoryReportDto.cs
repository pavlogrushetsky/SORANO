using System.Collections.Generic;

namespace SORANO.BLL.Dtos.ReportDtos
{
    public class InventoryReportDto
    {
        public Dictionary<string, IEnumerable<LocationGoodsDto>> LocationGoods { get; set; }
    }
}
