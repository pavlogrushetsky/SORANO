using System;

namespace SORANO.BLL.Dtos
{
    public class UserActivityDto
    {
        public DateTime DateTime { get; set; }

        public string EntityName { get; set; }

        public int EntityID { get; set; }

        public UserActivityType Type { get; set; }
    }
}