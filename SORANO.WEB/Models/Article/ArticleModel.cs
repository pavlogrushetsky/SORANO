namespace SORANO.WEB.Models.Article
{
    public class ArticleModel
    {
        public int ID { get; set; }       

        public string Name { get; set; }

        public string Description { get; set; }

        public string Producer { get; set; }

        public string Code { get; set; }

        public string Barcode { get; set; }

        public ArticleTypeModel Type { get; set; }
    }
}