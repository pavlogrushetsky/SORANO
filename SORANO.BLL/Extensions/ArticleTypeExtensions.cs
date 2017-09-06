using System.Linq;
using SORANO.BLL.DTOs;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class ArticleTypeExtensions
    {
        public static ArticleTypeDto ToDto(this ArticleType model)
        {
            var dto = new ArticleTypeDto
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                ParentTypeID = model.ParentType?.ID,
                ParentType = model.ParentType?.ToDto(),
                ChildTypes = model.ChildTypes.Select(t => t.ToDto()),
                Articles = model.Articles.Select(a => a.ToDto())
            };

            dto.MapDetails(model);
            dto.CanBeDeleted = model.Articles.All(a => a.IsDeleted) && !model.IsDeleted;

            return dto;
        }
    }
}