using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.User
{
    public class UserDetailsViewModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Роли")]
        public IEnumerable<string> Roles { get; set; }

        [Display(Name = "Разоешённые места")]
        public IEnumerable<string> Locations { get; set; }

        [Display(Name = "Статус")]
        public bool IsBlocked { get; set; }

        [Display(Name = "Действия")]
        public IEnumerable<UserActivityViewModel> Activities { get; set; }
    }
}
