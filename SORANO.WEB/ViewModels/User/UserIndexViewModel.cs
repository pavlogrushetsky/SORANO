﻿using System.Collections.Generic;

namespace SORANO.WEB.ViewModels.User
{
    public class UserIndexViewModel
    {
        public int ID { get; set; }

        public string Login { get; set; }

        public string Description { get; set; }

        public bool IsBlocked { get; set; }

        public bool CanBeBlocked { get; set; }

        public bool CanBeDeleted { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
