using System.ComponentModel.DataAnnotations;
using SORANO.WEB.Models.ArticleType;

namespace SORANO.WEB.Models.Article
{
    public class ArticleModel : EntityBaseModel
    {     
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Необходимо указать название артикула")]
        [MaxLength(500, ErrorMessage = "Длина названия не должна превышать 500 символов")]
        [MinLength(5, ErrorMessage = "Длина названия должна содержать не менее 5 символов")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        [MaxLength(1000, ErrorMessage = "Длина описания не должна превышать 1000 символов")]
        public string Description { get; set; }

        [Display(Name = "Производитель")]
        [MaxLength(1000, ErrorMessage = "Длина наименования производителя не должна превышать 200 символов")]
        public string Producer { get; set; }

        [Display(Name = "Код")]
        [MaxLength(1000, ErrorMessage = "Длина кода не должна превышать 50 символов")]
        public string Code { get; set; }

        [Display(Name = "Штрих-код")]
        [MaxLength(1000, ErrorMessage = "Длина штрих-кода не должна превышать 50 символов")]
        public string Barcode { get; set; }

        [Display(Name = "Тип артикулов")]
        public ArticleTypeModel Type { get; set; }
    }
}