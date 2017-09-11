using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class GoodsExtensions
    {
        public static GoodsDto ToDto(this Goods model)
        {
            return new GoodsDto
            {

            };
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