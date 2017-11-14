using System.Collections.Generic;
using SORANO.WEB.ViewModels.Recommendation;

namespace SORANO.WEB.ViewModels.Goods
{
    public class GoodsIndexViewModel
    {
        public int ArticleID { get; set; }

        public string ArticleName { get; set; }

        public string DeliveryID { get; set; }

        public string DeliveryBillNumber { get; set; }

        public string DeliveryPrice { get; set; }

        public string Currency { get; set; }

        public int StorageID { get; set; }

        public string StorageName { get; set; }

        public bool IsSold { get; set; }

        public int Quantity { get; set; }

        public IList<RecommendationViewModel> Recommendations { get; set; }
    }
}
