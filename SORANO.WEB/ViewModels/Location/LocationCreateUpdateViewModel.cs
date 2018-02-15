using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.Location
{
    public class LocationCreateUpdateViewModel : BaseCreateUpdateViewModel
    {
        [Display(Name = "Название *")]
        public string Name { get; set; }

        public string ReturnPath { get; set; }

        [Display(Name = "Описание")]
        public string Comment { get; set; }

        [Display(Name = "Тип *")]
        public int TypeID { get; set; }

        public string TypeName { get; set; }
    }
}