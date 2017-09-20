using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.Location
{
    public class LocationCreateUpdateViewModel : BaseCreateUpdateViewModel
    {
        public string Name { get; set; }

        public string ReturnPath { get; set; }

        public string Comment { get; set; }

        public int TypeID { get; set; }
    }
}