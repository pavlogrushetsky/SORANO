using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class GoodsChangeLocationModel
    {
        public int DeliveryItemID { get; set; }

        [Display(Name = "Количество, шт.")]
        public int Count { get; set; }

        [Display(Name = "Максимальное количество, шт.")]
        public int MaxCount { get; set; }

        [Display(Name = "Место")]
        public string TargetLocationID { get; set; }

        public int CurrentLocationID { get; set; }

        public string ReturnUrl { get; set; }
    }
}
