namespace SORANO.WEB.ViewModels.Goods
{
    public class GoodsItemViewModel
    {
        public string GoodsIds { get; set; }

        public int ArticleID { get; set; }
       
        public string ArticleName { get; set; }

        public string ArticleDescription { get; set; }

        public int ArticleTypeID { get; set; }

        public string ArticleTypeName { get; set; }

        public int LocationID { get; set; }

        public string LocationName { get; set; }

        public string RecommendedPrice { get; set; }

        public bool IsSold { get; set; }

        public int Quantity { get; set; }

        public string ImagePath { get; set; }
    }
}