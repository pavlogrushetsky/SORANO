using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class VisitExtensions
    {
        public static Visit ToEntity(this VisitDto dto)
        {
            return new Visit
            {
                ID = dto.ID,
                Code = dto.Code,
                Date = dto.Date,
                LocationID = dto.LocationID
            };
        }
    }
}