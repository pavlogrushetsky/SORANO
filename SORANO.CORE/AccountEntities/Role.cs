using System.Collections.Generic;

namespace SORANO.CORE.AccountEntities
{
    /// <summary>
    /// User role
    /// </summary>
    public class Role : Entity
    {
        /// <summary>
        /// Name of the role
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the role
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Users of the role
        /// </summary>
        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}