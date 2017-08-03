using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class SupplierModel : EntityBaseModel
    {
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        public string ReturnPath { get; set; }
    }
}
