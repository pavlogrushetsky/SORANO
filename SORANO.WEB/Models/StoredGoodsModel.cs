using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class StoredGoodsModel
    {
        [Display(Name = "Поставка")]
        public int DeliveryID { get; set; }

        [Display(Name = "Поставка")]
        public string BillNumber { get; set; }

        [Display(Name = "Артикул")]
        public int ArticleID { get; set; }

        [Display(Name = "Артикул")]
        public string Article { get; set; }

        [Display(Name = "Кол-во, шт.")]
        public int Quantity { get; set; }

        [Display(Name = "Цена поставки")]
        public string DeliveredPrice { get; set; }
    }
}
