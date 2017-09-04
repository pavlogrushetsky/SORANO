﻿namespace SORANO.BLL.DTOs
{
    public class ArticleDto : BaseDto
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Producer { get; set; }

        public string Code { get; set; }

        public string Barcode { get; set; }

        public int TypeID { get; set; }

        public ArticleTypeDto Type { get; set; }
    }
}