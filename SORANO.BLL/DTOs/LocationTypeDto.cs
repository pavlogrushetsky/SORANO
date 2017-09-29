using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class LocationTypeDto : BaseDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<LocationDto> Locations { get; set; }
    }
}