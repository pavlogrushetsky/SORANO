using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class SaleItemDto : BaseDto
    {
        public int SaleID { get; set; }

        public decimal? Price { get; set; }

        public SaleDto Sale { get; set; }

        public IEnumerable<GoodsDto> Goods { get; set; }
    }
}
