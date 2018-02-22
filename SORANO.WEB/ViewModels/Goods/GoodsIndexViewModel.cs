using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.Goods
{
    public class GoodsIndexViewModel
    {
        public int ArticleID { get; set; }

        [Display(Name = "Артикул:")]
        public string ArticleName { get; set; }

        public int ArticleTypeID { get; set; }

        [Display(Name = "Тип артикула:")]
        public string ArticleTypeName { get; set; }

        public int LocationID { get; set; }

        [Display(Name = "Склад:")]
        public string LocationName { get; set; }

        [Display(Name = "Поиск:")]
        public string SearchTerm { get; set; }

        [Display(Name = "Статус:")]
        public int Status { get; set; }

        [Display(Name = "Отображение:")]
        public bool ShowByPiece { get; set; }

        [Display(Name = "Кол-во на странице:")]
        public int ShowNumber { get; set; }      
    }
}
