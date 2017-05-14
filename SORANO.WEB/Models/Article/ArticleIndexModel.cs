using System.Collections.Generic;
using SORANO.CORE.StockEntities;

namespace SORANO.WEB.Models.Article
{
    public class ArticleIndexModel
    {
        public IEnumerable<ArticleTableModel> Table { get; set; }

        public IEnumerable<ArticleType> Tree { get; set; }
    }
}