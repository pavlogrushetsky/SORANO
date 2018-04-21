using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class DeliveryItemsDto
    {
        public IEnumerable<DeliveryItemDto> Items { get; set; }

        public DeliveryItemsSummaryDto Summary { get; set; }
    }
}