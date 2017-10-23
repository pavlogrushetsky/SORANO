using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.User
{
    public class UserBlockViewModel
    {
        public int ID { get; set; }

        public bool IsBlocked { get; set; }

        public bool CanBeBlocked { get; set; }

        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }
    }
}
