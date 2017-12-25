using System.Collections.Generic;

namespace SORANO.CORE.StockEntities
{
    public class Attachment : StockEntity
    {
        public int AttachmentTypeID { get; set; }

        public string FullPath { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual AttachmentType Type { get; set; }

        public virtual ICollection<StockEntity> ParentEntities { get; set; } = new HashSet<StockEntity>();
    }
}