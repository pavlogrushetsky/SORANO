namespace SORANO.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ReimplementedBarcodeUniqueIndex : DbMigration
    {
        string indexName = "IX_UQ_Article_Barcode";
        string tableName = "Articles";
        string columnName = "Barcode";

        public override void Up()
        {
            DropIndex("dbo.Articles", "IX_Article_Barcode");            

            Sql(string.Format(@"CREATE UNIQUE NONCLUSTERED INDEX {0} ON {1}({2}) WHERE {2} IS NOT NULL;", indexName, tableName, columnName));
        }
        
        public override void Down()
        {
            DropIndex(tableName, indexName);

            CreateIndex("dbo.Articles", "Barcode", unique: true, name: "IX_Article_Barcode");
        }
    }
}
