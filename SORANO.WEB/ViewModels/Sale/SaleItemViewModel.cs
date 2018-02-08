using System.Collections.Generic;

namespace SORANO.WEB.ViewModels.Sale
{
    public class SaleItemViewModel
    {
        public int GoodsId { get; set; }

        public int ArticleId { get; set; }

        public string ArticleName { get; set; }

        public int ArticleTypeId { get; set; }

        public string ArticleTypeName { get; set; }

        public int Quantity { get; set; } = 1;

        public string Price { get; set; } = "0.0";

        public bool IsSelected { get; set; }

        public List<SaleItemRecommendationViewModel> Recommendations { get; set; } = new List<SaleItemRecommendationViewModel>();
    }
}