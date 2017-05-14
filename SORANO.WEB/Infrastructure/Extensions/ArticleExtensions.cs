using SORANO.CORE.StockEntities;
using SORANO.WEB.Models.Article;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class ArticleExtensions
    {
        public static ArticleTableModel ToTableModel(this Article article)
        {
            return new ArticleTableModel
            {
                ID = article.ID,
                Code = article.Code,
                Name = article.Name,
                Producer = article.Producer,
                Type = article.Type?.Name
            };
        }

        public static void FromCreateModel(this ArticleType articleType, ArticleTypeCreateModel model)
        {
            articleType.Name = model.Name;
            articleType.Description = model.Description;
            articleType.ParentTypeId = model.ParentType > 0 ? (int?)model.ParentType : null;
        }
    }
}