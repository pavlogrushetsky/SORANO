using System.Collections.Generic;
using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.Goods
{
    public class GoodsRecommendationsViewModel : BaseCreateUpdateViewModel
    {
        public IList<int> Ids { get; set; }

        public string ArticleName { get; set; }

        public string ArticleDescription { get; set; }

        public string ArticleTypeName { get; set; }

        public string LocationName { get; set; }

        public string DeliveryPrice { get; set; }

        public string Currency { get; set; }

        public int Quantity { get; set; }

        public bool HasMainPicture => !string.IsNullOrWhiteSpace(MainPicture.FullPath);
    }
}