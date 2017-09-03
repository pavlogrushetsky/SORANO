using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels
{
    public class UserModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }       

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        public string RepeatPassword { get; set; }

        [Display(Name = "Роли")]
        public IEnumerable<string> RoleIDs { get; set; }

        [Display(Name = "Роли")]
        public List<string> Roles { get; set; } = new List<string>();

        [Display(Name = "Продажи")]
        public List<UserSaleModel> Sales { get; set; } = new List<UserSaleModel>();

        [Display(Name = "Действия")]
        public List<UserActivityModel> Activities { get; set; } = new List<UserActivityModel>();

        [Display(Name = "Статус")]
        public bool IsBlocked { get; set; }

        public bool CanBeDeleted { get; set; }

        public bool CanBeModified { get; set; }

        public bool CanBeBlocked { get; set; }
    }
}