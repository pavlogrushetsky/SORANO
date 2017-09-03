using System.Collections.Generic;

namespace SORANO.WEB.ViewModels
{
    public class GoodsIndexModel
    {
        public int ArticleId { get; set; }

        public string ArticleName { get; set; }

        public string ArticleImage { get; set; }  
        
        public List<GoodsGroupModel> Goods { get; set; }
    }

    public class GoodsGroupModel
    {
        public int Count { get; set; }

        public int LocationId { get; set; }

        public string LocationName { get; set; }

        public int DeliveryId { get; set; }

        public string BillNumber { get; set; }

        public string DeliveryPrice { get; set; }
    }
}