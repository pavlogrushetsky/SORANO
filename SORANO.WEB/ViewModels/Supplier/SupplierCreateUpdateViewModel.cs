using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.Supplier
{
    public class SupplierCreateUpdateViewModel : BaseCreateUpdateViewModel
    {
        [Display(Name = "Название *")]
        public string Name { get; set; }

        public string ReturnPath { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }
    }
}