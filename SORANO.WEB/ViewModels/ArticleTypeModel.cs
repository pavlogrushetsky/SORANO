using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels
{
    public class ArticleTypeModel : EntityBaseModel
    {
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Родительский тип")]
        public string ParentTypeID { get; set; }

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