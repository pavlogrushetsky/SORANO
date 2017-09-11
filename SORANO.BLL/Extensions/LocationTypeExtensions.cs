using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class LocationTypeExtensions
    {
        public static LocationTypeDto ToDto(this LocationType model)
        {
            return new LocationTypeDto
            {

            };
        }

        public static LocationType ToEntity(this LocationTypeDto dto)
        {
            return new LocationType
            {

            };
        }

        public static void UpdateFields(this LocationType existentLocationType, LocationType newLocationType)
        {
            existentLocationType.Name = newLocationType.Name;
            existentLocationType.Description = newLocationType.Description;
        }
    }
}