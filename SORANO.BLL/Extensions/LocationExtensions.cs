using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class LocationExtensions
    {
        public static LocationDto ToDto(this Location model)
        {
            return new LocationDto
            {

            };
        }

        public static Location ToEntity(this LocationDto dto)
        {
            return new Location
            {

            };
        }

        public static void UpdateFields(this Location existentLocation, Location newLocation)
        {
            existentLocation.Name = newLocation.Name;
            existentLocation.Comment = newLocation.Comment;
        }
    }
}
