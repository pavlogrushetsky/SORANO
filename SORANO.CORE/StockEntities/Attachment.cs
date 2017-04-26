using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    /// <summary>
    /// Attachment of an entity
    /// </summary>
    public class Attachment : StockEntity
    {
        /// <summary>
        /// Unique identifier of the attachment type
        /// </summary>
        public int AttachmentTypeID { get; set; }

        /// <summary>
        /// Full path of the attachment
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Name of the attachment file
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the attachment
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Type of the attachment
        /// </summary>
        public virtual AttachmentType Type { get; set; }

        /// <summary>
        /// Entities with this attachment
        /// </summary>
        public virtual ICollection<StockEntity> ParentEntities { get; set; } = new HashSet<StockEntity>();
    }
}