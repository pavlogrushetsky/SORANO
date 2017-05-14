using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SORANO.WEB.Models.Article
{
    public class ArticleTypeCreateModel
    {
        [Required(ErrorMessage = "Необходимо указать название типа артикулов")]
        [MaxLength(500, ErrorMessage = "Длина названия не должна превышать 500 символов")]
        [MinLength(5, ErrorMessage = "Длина названия должна содержать не менее 5 символов")]
        public string Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Длина описания не должна превышать 1000 символов")]
        public string Description { get; set; }

        public int ParentType { get; set; }

        public List<SelectListItem> AllTypes { get; set; } = new List<SelectListItem>();
    }
}