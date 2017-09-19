using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.ArticleType
{
    public class ArticleTypeDetailsViewModel : BaseDetailsViewModel
    {
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        public bool HasType { get; set; }

        public int TypeId { get; set; }

        [Display(Name = "Родительский тип")]
        public string TypeName { get; set; }

        public string TypeDescription { get; set; }
    }
}