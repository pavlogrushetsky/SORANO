using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;
using SORANO.WEB.ViewModels.DeliveryItem;

namespace SORANO.WEB.ViewModels.Article
{
    public class ArticleDetailsViewModel : BaseDetailsViewModel
    {
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Производитель")]
        public string Producer { get; set; }

        [Display(Name = "Код")]
        public string Code { get; set; }

        [Display(Name = "Штрих-код")]
        public string Barcode { get; set; }

        [Display(Name = "Рекомендованая цена")]
        public string RecommendedPrice { get; set; }

        public int TypeID { get; set; }

        [Display(Name = "Тип артикулов")]
        public string TypeName { get; set; }

        public string TypeDescription { get; set; }

        [Display(Name = "Позиции поставок")]
        public DeliveryItemTableViewModel Table { get; set; }
    }
}