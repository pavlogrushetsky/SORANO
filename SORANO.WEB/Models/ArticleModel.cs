using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class ArticleModel : EntityBaseModel
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
        public ArticleTypeModel Type { get; set; }       

        public string ReturnPath { get; set; }
    }
}