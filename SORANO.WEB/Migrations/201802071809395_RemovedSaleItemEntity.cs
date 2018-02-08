namespace SORANO.WEB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedSaleItemEntity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Goods", "SaleItemID", "dbo.SaleItems");
            DropForeignKey("dbo.SaleItems", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.SaleItems", "SaleID", "dbo.Sales");
            DropIndex("dbo.Goods", new[] { "SaleItemID" });
            DropIndex("dbo.SaleItems", new[] { "ID" });
            DropIndex("dbo.SaleItems", new[] { "SaleID" });
            AddColumn("dbo.Goods", "SaleID", c => c.Int());
            AddColumn("dbo.Goods", "Price", c => c.Decimal(precision: 38, scale: 2));
            AddColumn("dbo.Goods", "IsSold", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Goods", "SaleID");
            AddForeignKey("dbo.Goods", "SaleID", "dbo.Sales", "ID");
            DropColumn("dbo.Goods", "SaleItemID");
            DropTable("dbo.SaleItems");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SaleItems",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        SaleID = c.Int(nullable: false),
                        Price = c.Decimal(precision: 38, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Goods", "SaleItemID", c => c.Int());
            DropForeignKey("dbo.Goods", "SaleID", "dbo.Sales");
            DropIndex("dbo.Goods", new[] { "SaleID" });
            DropColumn("dbo.Goods", "IsSold");
            DropColumn("dbo.Goods", "Price");
            DropColumn("dbo.Goods", "SaleID");
            CreateIndex("dbo.SaleItems", "SaleID");
            CreateIndex("dbo.SaleItems", "ID");
            CreateIndex("dbo.Goods", "SaleItemID");
            AddForeignKey("dbo.SaleItems", "SaleID", "dbo.Sales", "ID");
            AddForeignKey("dbo.SaleItems", "ID", "dbo.StockEntities", "ID");
            AddForeignKey("dbo.Goods", "SaleItemID", "dbo.SaleItems", "ID");
        }
    }
}
