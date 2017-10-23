using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.User
{
    public class UserDeleteViewModel
    {
        public int ID { get; set; }

        public bool CanBeDeleted { get; set; }

        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }
    }
}
