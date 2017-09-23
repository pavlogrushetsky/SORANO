using System.Collections.Generic;
using System.Linq;
using SORANO.WEB.ViewModels.Article;

namespace SORANO.WEB.ViewModels.ArticleType
{
    public class ArticleTypeIndexViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public bool HasChildTypes => ChildTypes != null && ChildTypes.Any();

        public int ChildTypesCount => ChildTypes?.Count() ?? 0;

        public bool HasArticles => Articles != null && Articles.Any();

        public IEnumerable<ArticleTypeIndexViewModel> ChildTypes { get; set; }

        public IEnumerable<ArticleIndexViewModel> Articles { get; set; }
    }
}