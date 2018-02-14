using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    public class ArticleType : StockEntity
    {
        public int? ParentTypeId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ArticleType ParentType { get; set; }

        public virtual ICollection<ArticleType> ChildTypes { get; set; } = new HashSet<ArticleType>();

        public virtual ICollection<Article> Articles { get; set; } = new HashSet<Article>();
    }
}