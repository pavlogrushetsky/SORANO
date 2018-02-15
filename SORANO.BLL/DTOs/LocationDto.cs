using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class LocationDto : BaseDto
    {
        public int TypeID { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }

        public LocationTypeDto Type { get; set; }

        public IEnumerable<DeliveryDto> Deliveries { get; set; }
    }
}