using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.ArticleType
{
    public class ArticleTypeCreateUpdateViewModel : BaseCreateUpdateViewModel
    {
        [Display(Name = "Название *")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Родительский тип")]
        public int TypeID { get; set; }

        public string TypeName { get; set; }

        public string ReturnPath { get; set; }
    }
}