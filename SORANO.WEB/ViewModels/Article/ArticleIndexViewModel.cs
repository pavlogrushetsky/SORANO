namespace SORANO.WEB.ViewModels.Article
{
    public class ArticleIndexViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Producer { get; set; }

        public string Code { get; set; }

        public string Barcode { get; set; }

        public int TypeID { get; set; }

        public string TypeName { get; set; }

        public bool IsDeleted { get; set; }

        public bool CanBeDeleted { get; set; }

        public string Modified { get; set; }
    }
}