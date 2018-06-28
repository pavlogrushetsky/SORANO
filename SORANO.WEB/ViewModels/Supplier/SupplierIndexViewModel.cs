namespace SORANO.WEB.ViewModels.Supplier
{
    public class SupplierIndexViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public bool CanBeDeleted { get; set; }

        public string Modified { get; set; }

        public string ModifiedStandard { get; set; }
    }
}
