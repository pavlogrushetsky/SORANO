using System.Collections.Generic;

namespace SORANO.WEB.ViewModels.Sale
{
    public class SaleIndexViewModel
    {
        public SaleTableMode Mode { get; set; }

        public bool ShowLocation { get; set; }

        public IList<SaleViewModel> Items { get; set; }
    }
}