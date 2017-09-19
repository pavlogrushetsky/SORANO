using System.Collections.Generic;
using SORANO.WEB.ViewModels.Article;

namespace SORANO.WEB.ViewModels.ArticleType
{
    public class ArticleTypeIndexViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public bool HasChildTypes { get; set; }

        public int ChildTypesCount { get; set; }

        public bool HasArticles { get; set; }

        public IEnumerable<ArticleTypeIndexViewModel> ChildTypes { get; set; }

        public IEnumerable<ArticleIndexViewModel> Articles { get; set; }
    }
}