using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class LocationTypeModel : EntityBaseModel
    {
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Места")]
        public List<LocationModel> Locations { get; set; } = new List<LocationModel>();

        public string ReturnPath { get; set; }
    }
}
