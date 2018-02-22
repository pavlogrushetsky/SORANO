namespace SORANO.BLL.Dtos
{
    public class GoodsFilterCriteriaDto
    {
        public int ArticleID { get; set; }

        public int ArticleTypeID { get; set; }

        public int LocationID { get; set; }

        public string SearchTerm { get; set; }

        public int Status { get; set; }

        public bool ShowByPiece { get; set; }

        public int ShowNumber { get; set; }
    }
}