using System.Collections.Generic;

namespace SORANO.WEB.Models.ArticleType
{
    public class ArticleTypeSelectModel
    {
        public List<ArticleTypeModel> Types { get; set; }

        public ArticleTypeModel ArticleType { get; set; }

        public string ReturnUrl { get; set; }
    }
}