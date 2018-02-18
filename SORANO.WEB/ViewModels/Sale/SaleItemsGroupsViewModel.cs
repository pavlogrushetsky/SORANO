using System.Collections.Generic;

namespace SORANO.WEB.ViewModels.Sale
{
    public class SaleItemsGroupsViewModel
    {
        public SaleItemsSummaryViewModel Summary { get; set; }

        public string Warning { get; set; }

        public List<SaleItemsGroupViewModel> Groups { get; set; } = new List<SaleItemsGroupViewModel>();
    }
}