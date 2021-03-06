﻿using System.Collections.Generic;
using SORANO.CORE.StockEntities;

namespace SORANO.CORE.AccountEntities
{
    public class User : Entity
    {
        public string Description { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public bool IsBlocked { get; set; }

        public ICollection<Role> Roles { get; set; } = new HashSet<Role>();

        public ICollection<StockEntity> CreatedEntities { get; set; } = new HashSet<StockEntity>();

        public ICollection<StockEntity> ModifiedEntities { get; set; } = new HashSet<StockEntity>();

        public ICollection<StockEntity> DeletedEntities { get; set; } = new HashSet<StockEntity>();

        public ICollection<Sale> Sales { get; set; } = new HashSet<Sale>();

        public ICollection<Location> Locations { get; set; } = new HashSet<Location>();
    }
}