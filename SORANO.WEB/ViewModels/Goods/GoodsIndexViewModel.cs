using System.Collections.Generic;

namespace SORANO.WEB.ViewModels.Goods
{
    public class GoodsIndexViewModel
    {
        public int ArticleID { get; set; }

        public string ArticleName { get; set; }

        public int ArticleTypeID { get; set; }

        public string ArticleTypeName { get; set; }

        public int LocationID { get; set; }

        public string LocationName { get; set; }

        public string SearchTerm { get; set; }

        public bool ShowSold { get; set; }

        public IEnumerable<GoodsItemViewModel> Goods { get; set; }       
    }
}
