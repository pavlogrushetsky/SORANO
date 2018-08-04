using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    public class AttachmentType : StockEntity
    {
        public string Name { get; set; }

        public string Comment { get; set; }

        public string Extensions { get; set; }

        public ICollection<Attachment> TypeAttachments { get; set; } = new HashSet<Attachment>();
    }
}