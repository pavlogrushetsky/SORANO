using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.Location
{
    public class LocationDetailsViewModel : BaseDetailsViewModel
    {
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Comment { get; set; }

        public int TypeID { get; set; }

        [Display(Name = "Тип мест")]
        public string TypeName { get; set; }

        public string TypeDescription { get; set; }
    }
}