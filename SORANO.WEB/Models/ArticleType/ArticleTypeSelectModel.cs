using System.Collections.Generic;
using SORANO.WEB.Models.Article;

namespace SORANO.WEB.Models.ArticleType
{
    public class ArticleTypeSelectModel
    {
        public List<ArticleTypeModel> Types { get; set; }

        public ArticleTypeModel ArticleType { get; set; }

        public ArticleModel Article { get; set; }

        public string ReturnUrl { get; set; }
    }
}