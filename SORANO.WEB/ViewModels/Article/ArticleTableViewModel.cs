using System.Collections.Generic;

namespace SORANO.WEB.ViewModels.Article
{
    public class ArticleTableViewModel
    {
        public ArticleTableMode Mode { get; set; }

        public bool ShowDeleted { get; set; }

        public IList<ArticleViewModel> Articles { get; set; }
    }
}