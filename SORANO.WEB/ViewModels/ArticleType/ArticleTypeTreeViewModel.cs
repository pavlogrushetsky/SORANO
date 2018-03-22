using System.Collections.Generic;

namespace SORANO.WEB.ViewModels.ArticleType
{
    public class ArticleTypeTreeViewModel
    {
        public bool ShowDeleted { get; set; }

        public IList<ArticleTypeIndexViewModel> ArticleTypes { get; set; }
    }
}