using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class ArticleTypeModel : EntityBaseModel
    {
        [Required(ErrorMessage = "Необходимо указать название типа артикулов")]
        [MaxLength(500, ErrorMessage = "Длина названия не должна превышать 500 символов")]
        [MinLength(5, ErrorMessage = "Длина названия должна содержать не менее 5 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Длина описания не должна превышать 1000 символов")]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Родительский тип")]
        public ArticleTypeModel ParentType { get; set; }

        [Display(Name = "Вложенные типы")]
        public IList<ArticleTypeModel> ChildTypes { get; set; } = new List<ArticleTypeModel>();

        [Display(Name = "Артикулы")]
        public IList<ArticleModel> Articles { get; set; } = new List<ArticleModel>();

        public bool IsSelected { get; set; }

        public string ReturnPath { get; set; }
    }
}