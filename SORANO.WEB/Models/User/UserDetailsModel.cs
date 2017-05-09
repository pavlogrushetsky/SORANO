using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models.User
{
    /// <summary>
    /// User model to display details
    /// </summary>
    public class UserDetailsModel
    {
        /// <summary>
        /// User identifier
        /// </summary>
        [Display(Name = "ID")]
        public int ID { get; set; }

        /// <summary>
        /// User login
        /// </summary>
        [Display(Name = "Логин")]
        public string Login { get; set; }

        /// <summary>
        /// User description
        /// </summary>
        [Display(Name = "Описание")]
        public string Description { get; set; }

        /// <summary>
        /// User roles
        /// </summary>
        [Display(Name = "Роли")]
        public List<string> Roles { get; set; }

        /// <summary>
        /// Specifies whether user is blocked or not
        /// </summary>
        [Display(Name = "Статус")]
        public bool IsBlocked { get; set; }

        /// <summary>
        /// Specifies whether user can be deleted or not
        /// </summary>
        public bool CanBeDeleted { get; set; }

        /// <summary>
        /// Specifies whether user can be blocked or not
        /// </summary>
        public bool CanBeBlocked { get; set; }

        /// <summary>
        /// User sales
        /// </summary>
        [Display(Name = "Продажи")]
        public List<UserSaleModel> Sales { get; set; }

        /// <summary>
        /// User activities
        /// </summary>
        [Display(Name = "Действия")]
        public List<UserActivityModel> Activities { get; set; }
    }
}
