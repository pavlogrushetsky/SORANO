using System.Collections.Generic;

namespace SORANO.BLL.DTOs
{
    public class AllGoodsDTO
    {
        public int ArticleId { get; set; }

        public string ArticleName { get; set; }

        public string ArticleImage { get; set; }

        public List<GoodsGroupDTO> Goods { get; set; }
    }
}