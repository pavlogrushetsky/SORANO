using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    /// <summary>
    /// Type of an attachment
    /// </summary>
    public class AttachmentType : StockEntity
    {
        /// <summary>
        /// Name of the type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Comment for the type
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// String to filter acceptable files' types
        /// </summary>
        public string Extensions { get; set; }

        /// <summary>
        /// Attachment of this type
        /// </summary>
        public virtual ICollection<Attachment> TypeAttachments { get; set; } = new HashSet<Attachment>();
    }
}