using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    /// <summary>
    /// Type of goods article
    /// </summary>
    public class ArticleType : StockEntity
    {
        /// <summary>
        /// Unique identifier of the parent type
        /// </summary>
        public int? ParentTypeId { get; set; }

        /// <summary>
        /// Name of the type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the type
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Parent type
        /// </summary>
        public virtual ArticleType ParentType { get; set; }

        /// <summary>
        /// Articles of the type
        /// </summary>
        public virtual ICollection<Article> Articles { get; set; } = new HashSet<Article>();
    }
}