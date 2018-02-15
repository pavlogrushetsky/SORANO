using System.Collections.Generic;

namespace SORANO.WEB.ViewModels.Article
{
    public class ArticleIndexViewModel
    {
        public ArticleTableMode Mode { get; set; }

        public IList<ArticleViewModel> Articles { get; set; }
    }
}