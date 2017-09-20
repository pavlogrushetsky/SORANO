using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.LocationType
{
    public class LocationTypeCreateUpdateViewModel : BaseCreateUpdateViewModel
    {
        public string Name { get; set; }

        public string ReturnPath { get; set; }

        public string Description { get; set; }
    }
}