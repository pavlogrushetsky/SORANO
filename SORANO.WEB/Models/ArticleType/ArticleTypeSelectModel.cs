using System.Collections.Generic;

namespace SORANO.WEB.Models.ArticleType
{
    public class ArticleTypeSelectModel
    {
        public List<ArticleTypeModel> Types { get; set; }

        public ArticleTypeModel CurrentType { get; set; }

        public string ReturnUrl { get; set; }
    }
}