using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class StorageExtensions
    {
        public static StorageDto ToDto(this Storage model)
        {
            return new StorageDto
            {
                ID = model.ID,
                GoodsID = model.GoodsID,
                LocationID = model.LocationID,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                Location = model.Location.ToDto()
            };
        }
    }
}