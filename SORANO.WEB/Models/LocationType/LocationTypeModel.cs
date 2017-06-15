using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SORANO.WEB.Models.Location;

namespace SORANO.WEB.Models.LocationType
{
    public class LocationTypeModel : EntityBaseModel
    {
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Необходимо указать название типа мест")]
        [MaxLength(200, ErrorMessage = "Длина названия не должна превышать 200 символов")]
        [MinLength(5, ErrorMessage = "Длина названия должна содержать не менее 5 символов")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        [MaxLength(1000, ErrorMessage = "Длина описания не должна превышать 1000 символов")]
        public string Description { get; set; }

        [Display(Name = "Места")]
        public List<LocationModel> Locations { get; set; } = new List<LocationModel>();       
    }
}
