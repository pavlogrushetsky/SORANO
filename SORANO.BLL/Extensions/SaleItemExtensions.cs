using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;
using System.Linq;

namespace SORANO.BLL.Extensions
{
    internal static class SaleItemExtensions
    {
        public static SaleItemDto ToDto(this SaleItem model)
        {
            var dto = new SaleItemDto
            {
                ID = model.ID,
                SaleID = model.SaleID,
                Sale = model.Sale.ToDto(),
                Goods = model.Goods.Select(g => g.ToDto()),
                Price = model.Price
            };

            dto.MapDetails(model);
            dto.CanBeDeleted = !(model.Sale.IsSubmitted || model.IsDeleted);

            return dto;
        }

        public static SaleItem ToEntity(this SaleItemDto dto)
        {
            var entity = new SaleItem
            {

            };

            return entity;
        }

        public static void UpdateFields(this SaleItem existentSaleItem, SaleItem newSaleItem)
        {

        }
    }
}
