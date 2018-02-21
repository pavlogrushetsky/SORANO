using System.Collections.Generic;

namespace SORANO.WEB.ViewModels.Sale
{
    public class SaleItemViewModel
    {
        public int GoodsId { get; set; }

        public string Price { get; set; }

        public bool IsSelected { get; set; }

        public List<SaleItemRecommendationViewModel> Recommendations { get; set; } = new List<SaleItemRecommendationViewModel>();
    }
}