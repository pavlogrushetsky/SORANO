using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class UserDto
    {
        public int ID { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string Description { get; set; }

        public bool IsBlocked { get; set; }

        public IEnumerable<RoleDto> Roles { get; set; }

        public IEnumerable<GoodsDto> SoldGoods { get; set; }
    }
}