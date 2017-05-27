using SORANO.CORE.StockEntities;
using SORANO.WEB.Models.Article;
using SORANO.WEB.Models.ArticleType;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class ArticleExtensions
    {
        public static ArticleModel ToModel(this Article article, ArticleTypeModel type)
        {
            return new ArticleModel
            {
                ID = article.ID,
                Code = article.Code,
                Name = article.Name,
                Producer = article.Producer,
                Description = article.Description,
                Barcode = article.Barcode,
                Type = type
            };
        }

        public static void FromCreateModel(this ArticleType articleType, ArticleTypeModel model)
        {
            articleType.Name = model.Name;
            articleType.Description = model.Description;
            articleType.ParentTypeId = model.ParentType.ID > 0 ? (int?)model.ParentType.ID : null;
        }
    }
}