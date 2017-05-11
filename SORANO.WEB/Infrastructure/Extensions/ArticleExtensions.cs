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
                Description = article.Description,
                Producer = article.Producer,
                Type = article.Type?.Name
            };
        }
    }
}