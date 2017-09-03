using System.Collections.Generic;

namespace SORANO.WEB.ViewModels
{
    public class ArticleTypeSelectModel
    {
        public List<ArticleTypeModel> Types { get; set; } = new List<ArticleTypeModel>();

        public ArticleTypeModel CurrentType { get; set; }

        public string ReturnUrl { get; set; }
    }
}