using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class SupplierDto : BaseDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<DeliveryDto> Deliveries { get; set; }
    }
}