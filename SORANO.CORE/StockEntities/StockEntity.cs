using System;
using System.Collections.Generic;
using SORANO.CORE.AccountEntities;

namespace SORANO.CORE.StockEntities
{
    public abstract class StockEntity : Entity
    {
        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public int? DeletedBy { get; set; }

        public User CreatedByUser { get; set; }

        public User ModifiedByUser { get; set; }

        public User DeletedByUser { get; set; }

        public ICollection<Recommendation> Recommendations { get; set; } = new HashSet<Recommendation>();

        public ICollection<Attachment> Attachments { get; set; } = new HashSet<Attachment>();
    }
}