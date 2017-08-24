using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class SaleModel
    {
        public int ArticleID { get; set; }

        [Display(Name = "Артикул")]
        public string ArticleName { get; set; }

        public bool IsArticleEditable { get; set; }

        [Display(Name = "Количество, шт.")]
        public int Count { get; set; }

        [Display(Name = "Максимально количество, шт.")]
        public int? MaxCount { get; set; }

        public int LocationID { get; set; }

        [Display(Name = "Место")]
        public string LocationName { get; set; }

        public bool IsLocationEditable { get; set; }

        public int ClientID { get; set; }

        [Display(Name = "Клиент")]
        public string ClientName { get; set; }

        [Display(Name = "Цена единицы товара, ₴")]
        public string SalePrice { get; set; }

        [Display(Name = "Общая сумма")]
        public string TotalPrice { get; set; }

        [Display(Name = "Дата покупки/продажи")]
        public string SaleDate { get; set; }

        public string ReturnUrl { get; set; }
    }
}