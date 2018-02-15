using System.Collections.Generic;

namespace SORANO.CORE.AccountEntities
{
    public class Role : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}