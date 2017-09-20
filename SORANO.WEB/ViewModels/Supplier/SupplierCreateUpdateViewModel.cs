using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.Supplier
{
    public class SupplierCreateUpdateViewModel : BaseCreateUpdateViewModel
    {
        public string Name { get; set; }

        public string ReturnPath { get; set; }

        public string Description { get; set; }
    }
}