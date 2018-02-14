using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class AllGoodsDto
    {
        public int ArticleId { get; set; }

        public string ArticleName { get; set; }

        public string ArticleImage { get; set; }

        public List<GoodsGroupDto> Goods { get; set; }
    }
}