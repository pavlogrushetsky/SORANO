using System.Collections.Generic;

namespace SORANO.BLL.DTOs
{
    public class ArticleDto
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Producer { get; set; }

        public string Code { get; set; }

        public string Barcode { get; set; }

        public IEnumerable<RecommendationDto> Recommendations { get; set; }
    }
}