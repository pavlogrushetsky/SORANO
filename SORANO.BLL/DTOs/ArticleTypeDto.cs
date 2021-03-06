﻿using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class ArticleTypeDto : BaseDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int? TypeID { get; set; }

        public ArticleTypeDto Type { get; set; }

        public IEnumerable<ArticleTypeDto> ChildTypes { get; set; }

        public IEnumerable<ArticleDto> Articles { get; set; }
    }
}