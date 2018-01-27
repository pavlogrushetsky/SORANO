namespace SORANO.WEB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSaleEntities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Goods", "ClientID", "dbo.Clients");
            DropForeignKey("dbo.Goods", "SoldBy", "dbo.Users");
            DropForeignKey("dbo.Goods", "SaleLocationID", "dbo.Locations");
            DropIndex("dbo.Goods", new[] { "ClientID" });
            DropIndex("dbo.Goods", new[] { "SoldBy" });
            DropIndex("dbo.Goods", new[] { "SaleLocationID" });
            CreateTable(
                "dbo.SaleItems",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        SaleID = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 38, scale: 2),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Goods", t => t.ID)
                .ForeignKey("dbo.Sales", t => t.SaleID)
                .Index(t => t.ID)
                .Index(t => t.SaleID);
            
            CreateTable(
                "dbo.Sales",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        ClientID = c.Int(),
                        LocationID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        IsSubmitted = c.Boolean(nullable: false),
                        Date = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .ForeignKey("dbo.Clients", t => t.ClientID)
                .ForeignKey("dbo.Locations", t => t.LocationID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.ID)
                .Index(t => t.ClientID)
                .Index(t => t.LocationID)
                .Index(t => t.UserID);
            
            DropColumn("dbo.Goods", "ClientID");
            DropColumn("dbo.Goods", "SalePrice");
            DropColumn("dbo.Goods", "SaleDate");
            DropColumn("dbo.Goods", "SoldBy");
            DropColumn("dbo.Goods", "SaleLocationID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Goods", "SaleLocationID", c => c.Int());
            AddColumn("dbo.Goods", "SoldBy", c => c.Int());
            AddColumn("dbo.Goods", "SaleDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.Goods", "SalePrice", c => c.Decimal(precision: 38, scale: 2));
            AddColumn("dbo.Goods", "ClientID", c => c.Int());
            DropForeignKey("dbo.Sales", "UserID", "dbo.Users");
            DropForeignKey("dbo.Sales", "LocationID", "dbo.Locations");
            DropForeignKey("dbo.Sales", "ClientID", "dbo.Clients");
            DropForeignKey("dbo.Sales", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.SaleItems", "SaleID", "dbo.Sales");
            DropForeignKey("dbo.SaleItems", "ID", "dbo.Goods");
            DropIndex("dbo.Sales", new[] { "UserID" });
            DropIndex("dbo.Sales", new[] { "LocationID" });
            DropIndex("dbo.Sales", new[] { "ClientID" });
            DropIndex("dbo.Sales", new[] { "ID" });
            DropIndex("dbo.SaleItems", new[] { "SaleID" });
            DropIndex("dbo.SaleItems", new[] { "ID" });
            DropTable("dbo.Sales");
            DropTable("dbo.SaleItems");
            CreateIndex("dbo.Goods", "SaleLocationID");
            CreateIndex("dbo.Goods", "SoldBy");
            CreateIndex("dbo.Goods", "ClientID");
            AddForeignKey("dbo.Goods", "SaleLocationID", "dbo.Locations", "ID");
            AddForeignKey("dbo.Goods", "SoldBy", "dbo.Users", "ID");
            AddForeignKey("dbo.Goods", "ClientID", "dbo.Clients", "ID");
        }
    }
}
