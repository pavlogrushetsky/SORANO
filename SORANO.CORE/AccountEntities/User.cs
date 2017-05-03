﻿using System.Collections.Generic;
using SORANO.CORE.StockEntities;

namespace SORANO.CORE.AccountEntities
{
    /// <summary>
    /// User
    /// </summary>
    public class User : Entity
    {
        /// <summary>
        /// Description of the user
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Login of the user
        /// </summary>
        public string Login { get; set; }

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