using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models.User
{
    /// <summary>
    /// User model to display for block action
    /// </summary>
    public class UserBlockModel
    {
        /// <summary>
        /// User identifier
        /// </summary>
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
        /// Specifies whether user can be blocked or not
        /// </summary>
        public bool CanBeBlocked { get; set; }

        /// <summary>
        /// Specifies whether user can is blocked or not
        /// </summary>
        public bool IsBlocked { get; set; }
    }
}
