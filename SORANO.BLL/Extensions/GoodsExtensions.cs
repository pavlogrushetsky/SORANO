using System.Linq;
using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class GoodsExtensions
    {
        public static GoodsDto ToDto(this Goods model)
        {
            var dto = new GoodsDto
            {
                ID = model.ID,
                DeliveryItemID = model.DeliveryItemID,
                SaleID = model.SaleID,
                Sale = model.Sale?.ToDto(),
                IsSold = model.IsSold,
                Price = model.Price,
                //ClientID = model.ClientID,TODO
                //SalePrice = model.SalePrice,
                //SaleDate = model.SaleDate,
                //SoldBy = model.SoldBy,
                //SaleLocationID = model.SaleLocationID,
                DeliveryItem = model.DeliveryItem.ToDto(),
                //Client = model.Client?.ToDto(),
                //SoldByUser = model.SoldByUser?.Login,
                //SaleLocation = model.SaleLocation?.ToDto(),
                Storages = model.Storages.OrderByDescending(s => s.FromDate).Select(s => s.ToDto()).ToList(),
                Quantity = 1
            };

            dto.MapDetails(model);
            dto.CanBeDeleted = false;

            return dto;
        }

        public static Goods ToEntity(this GoodsDto dto)
        {
            return new Goods
            {

            };
        }

        public static void UpdateFields(this Goods existentGoods, Goods newGoods)
        {
            
        }
    }
}