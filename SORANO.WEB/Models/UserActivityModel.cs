using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class UserActivityModel
    {
        /// <summary>
        /// Activity type
        /// </summary>
        [Display(Name = "Тип действия")]
        public string ActivityType { get; set; }

        /// <summary>
        /// Entity identifier
        /// </summary>
        [Display(Name = "ID сущности")]
        public int EntityID { get; set; }

        /// <summary>
        /// Entity type
        /// </summary>
        [Display(Name = "Тип сущности")]
        public string EntityType { get; set; }

        /// <summary>
        /// Activity date
        /// </summary>
        [Display(Name = "Дата")]
        public string Date { get; set; }
    }
}
