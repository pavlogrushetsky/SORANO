using System.Collections.Generic;

namespace SORANO.BLL.DTOs
{
    public class ArticleTypeDto : BaseDto
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? ParentTypeID { get; set; }

        public ArticleTypeDto ParentType { get; set; }

        public IEnumerable<ArticleTypeDto> ChildTypes { get; set; }

        public IEnumerable<ArticleDto> Articles { get; set; }
    }
}