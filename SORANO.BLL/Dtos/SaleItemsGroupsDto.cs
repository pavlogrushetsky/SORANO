using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class SaleItemsGroupsDto
    {
        public SaleItemsSummaryDto Summary { get; set; }

        public List<SaleItemsGroupDto> Groups { get; set; } = new List<SaleItemsGroupDto>();
    }
}