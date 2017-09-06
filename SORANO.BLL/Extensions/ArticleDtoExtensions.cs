using System.Linq;
using SORANO.BLL.DTOs;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class ArticleDtoExtensions
    {
        public static ArticleDto ToDto(this Article model)
        {
            var dto = new ArticleDto
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                Producer = model.Producer,
                Code = model.Code,
                Barcode = model.Barcode,
                TypeID = model.TypeID,
                Type = model.Type.ToDto()
            };

            dto.MapDetails(model);
            dto.CanBeDeleted = !model.DeliveryItems.Any() && !model.IsDeleted;

            return dto;
        }
    }
}