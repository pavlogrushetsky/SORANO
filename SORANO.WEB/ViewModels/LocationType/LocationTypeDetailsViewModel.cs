using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.LocationType
{
    public class LocationTypeDetailsViewModel : BaseDetailsViewModel
    {
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }
    }
}