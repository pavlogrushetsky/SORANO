namespace SORANO.WEB.ViewModels.Location
{
    public class LocationIndexViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }

        public int TypeID { get; set; }

        public string TypeName { get; set; }

        public bool IsDeleted { get; set; }

        public bool CanBeDeleted { get; set; }

        public string Modified { get; set; }
    }
}