using System;
using System.Collections.Generic;

namespace SORANO.CORE
{
    /// <summary>
    /// Base abstract class for all the entities of the domain model
    /// </summary>
    public abstract class StockEntity
    {
        /// <summary>
        /// Unique identifier of the entity
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Date and time of the entity creation
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Unique identifier of a user created the entity
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Date and time of the entity last modification
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Unique identifier of a user modified the entity
        /// </summary>
        public int ModifiedBy { get; set; }

        /// <summary>
        /// Date and time of the entity deletion
        /// </summary>
        public DateTime? DeletedDate { get; set; }

        /// <summary>
        /// Unique identifier of a user deleted the entity
        /// </summary>
        public int? DeletedBy { get; set; }

        /// <summary>
        /// Recommendations attached to the entity
        /// </summary>
        public virtual ICollection<Recommendation> Recommendations { get; set; } = new HashSet<Recommendation>();
    }
}