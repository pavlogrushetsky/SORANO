using System.Collections.Generic;

namespace SORANO.CORE.IdentityEntities
{
    /// <summary>
    /// User role
    /// </summary>
    public class Role : StockEntity
    {
        /// <summary>
        /// Name of the role
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Users of the role
        /// </summary>
        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}