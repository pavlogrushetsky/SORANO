namespace SORANO.WEB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OneToManyForSaleGoods : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SaleItems", "ID", "dbo.Goods");
            AddColumn("dbo.Sales", "TotalPrice", c => c.Decimal(precision: 38, scale: 2));
            AddColumn("dbo.Sales", "DollarRate", c => c.Decimal(precision: 38, scale: 2));
            AddColumn("dbo.Sales", "EuroRate", c => c.Decimal(precision: 38, scale: 2));
            AddColumn("dbo.Goods", "SaleItemID", c => c.Int());
            AlterColumn("dbo.SaleItems", "Price", c => c.Decimal(precision: 38, scale: 2));
            CreateIndex("dbo.Goods", "SaleItemID");
            AddForeignKey("dbo.Goods", "SaleItemID", "dbo.SaleItems", "ID");
            AddForeignKey("dbo.SaleItems", "ID", "dbo.StockEntities", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SaleItems", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.Goods", "SaleItemID", "dbo.SaleItems");
            DropIndex("dbo.Goods", new[] { "SaleItemID" });
            AlterColumn("dbo.SaleItems", "Price", c => c.Decimal(nullable: false, precision: 38, scale: 2));
            DropColumn("dbo.Goods", "SaleItemID");
            DropColumn("dbo.Sales", "EuroRate");
            DropColumn("dbo.Sales", "DollarRate");
            DropColumn("dbo.Sales", "TotalPrice");
            AddForeignKey("dbo.SaleItems", "ID", "dbo.Goods", "ID");
        }
    }
}
