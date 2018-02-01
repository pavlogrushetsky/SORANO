using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SORANO.WEB.ViewModels.User
{
    public class UserCreateUpdateViewModel
    {
        public int ID { get; set; }

        public bool IsUpdate { get; set; }

        public bool CanBeModified { get; set; }

        [Display(Name = "Логин *")]
        public string Login { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Пароль *")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Новый пароль")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Повторите пароль")]
        [DataType(DataType.Password)]
        public string RepeatPassword { get; set; }

        [Display(Name = "Роли *")]
        public IEnumerable<int> RoleIDs { get; set; }

        public IList<SelectListItem> Roles { get; set; }

        [Display(Name = "Разрешённые места")]
        public IEnumerable<int> LocationIds { get; set; }

        public IList<SelectListItem> LocationNames { get; set; }
    }
}
