using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.Location
{
    public class LocationDeleteViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Comment { get; set; }

        public bool CanBeDeleted { get; set; }
    }
}