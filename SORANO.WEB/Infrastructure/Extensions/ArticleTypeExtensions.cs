using System.Linq;
using SORANO.CORE.StockEntities;
using SORANO.WEB.Models.ArticleType;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class ArticleTypeExtensions
    {
        public static ArticleTypeModel ToModel(this ArticleType type, bool deep = true)
        {
            var model = new ArticleTypeModel
            {
                ID = type.ID,
                Name = type.Name,
                Description = type.Description,
                CanBeDeleted = !type.Articles.Any()
            };

            if (!deep)
            {
                return model;
            }

            model.ParentType = type.ParentType?.ToModel(false);
            model.Articles = type.Articles?.Select(a => a.ToModel(model)).ToList();

            return model;
        }

        public static void FromCreateModel(this ArticleType articleType, ArticleTypeModel model)
        {
            articleType.Name = model.Name;
            articleType.Description = model.Description;
            if (model.ParentType != null && model.ParentType.ID > 0)
            {
                articleType.ParentTypeId = model.ParentType.ID;
            }
            else
            {
                articleType.ParentTypeId = null;
            }
        }
    }
}