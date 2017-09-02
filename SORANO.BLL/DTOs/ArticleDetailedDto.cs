using System;

namespace SORANO.BLL.DTOs
{
    public class ArticleDetailedDto : ArticleDto
    {
        public bool IsDeleted { get; set; }

        public bool CanBeDeleted { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public DateTime? Deleted { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public string DeletedBy { get; set; }
    }
}