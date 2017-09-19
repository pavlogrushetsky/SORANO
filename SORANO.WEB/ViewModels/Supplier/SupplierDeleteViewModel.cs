using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.Supplier
{
    public class SupplierDeleteViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        public bool CanBeDeleted { get; set; }
    }
}
