using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    public class ArticleType : StockEntity
    {
        public int? ParentTypeId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ArticleType ParentType { get; set; }

        public ICollection<ArticleType> ChildTypes { get; set; } = new HashSet<ArticleType>();

        public ICollection<Article> Articles { get; set; } = new HashSet<Article>();
    }
}