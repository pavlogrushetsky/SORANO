namespace SORANO.WEB.ViewModels.LocationType
{
    public class LocationTypeIndexViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public bool CanBeDeleted { get; set; }
    }
}
