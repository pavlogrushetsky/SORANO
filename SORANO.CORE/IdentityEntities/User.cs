using System.Collections.Generic;
using SORANO.CORE.StockEntities;

namespace SORANO.CORE.IdentityEntities
{
    /// <summary>
    /// User
    /// </summary>
    public class User : StockEntity
    {
        /// <summary>
        /// Name of the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email address of the user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password of the user
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Specified whether the user is blocked or not
        /// </summary>
        public bool IsBlocked { get; set; }

        /// <summary>
        /// Roles of the user
        /// </summary>
        public virtual ICollection<Role> Roles { get; set; } = new HashSet<Role>();

        /// <summary>
        /// Entities created by the user
        /// </summary>
        public virtual ICollection<StockEntity> CreatedEntities { get; set; } = new HashSet<StockEntity>();

        /// <summary>
        /// Entities modified by the user
        /// </summary>
        public virtual ICollection<StockEntity> ModifiedEntities { get; set; } = new HashSet<StockEntity>();

        /// <summary>
        /// Entities deleted by the user
        /// </summary>
        public virtual ICollection<StockEntity> DeletedEntities { get; set; } = new HashSet<StockEntity>();

        /// <summary>
        /// Goods sold by the user
        /// </summary>
        public virtual ICollection<Goods> SoldGoods { get; set; } = new HashSet<Goods>();
    }
}