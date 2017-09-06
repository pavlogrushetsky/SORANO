using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;

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

        [Display(Name = "Тип артикулов")]
        public string TypeID { get; set; }

        [Display(Name = "Тип артикулов")]
        public ArticleTypeModel Type { get; set; }

        public string ReturnPath { get; set; }
    }
}