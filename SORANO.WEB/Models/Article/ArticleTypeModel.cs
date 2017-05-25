using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models.Article
{
    public class ArticleTypeModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Необходимо указать название типа артикулов")]
        [MaxLength(500, ErrorMessage = "Длина названия не должна превышать 500 символов")]
        [MinLength(5, ErrorMessage = "Длина названия должна содержать не менее 5 символов")]
        public string Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Длина описания не должна превышать 1000 символов")]
        public string Description { get; set; }

        public ArticleTypeModel ParentType { get; set; }

        public IList<ArticleTypeModel> ChildTypes { get; set; } = new List<ArticleTypeModel>();

        public IList<ArticleModel> Articles { get; set; } = new List<ArticleModel>();
    }
}