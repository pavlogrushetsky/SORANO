namespace SORANO.WEB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedVisitsAndVisitors : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Visitors",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Gender = c.Int(nullable: false),
                        AgeGroup = c.Int(nullable: false),
                        VisitID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .ForeignKey("dbo.Visits", t => t.VisitID)
                .Index(t => t.ID)
                .Index(t => t.VisitID);
            
            CreateTable(
                "dbo.Visits",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Code = c.String(nullable: false, maxLength: 200),
                        LocationID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .ForeignKey("dbo.Locations", t => t.LocationID)
                .Index(t => t.ID)
                .Index(t => t.LocationID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Visits", "LocationID", "dbo.Locations");
            DropForeignKey("dbo.Visits", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.Visitors", "VisitID", "dbo.Visits");
            DropForeignKey("dbo.Visitors", "ID", "dbo.StockEntities");
            DropIndex("dbo.Visits", new[] { "LocationID" });
            DropIndex("dbo.Visits", new[] { "ID" });
            DropIndex("dbo.Visitors", new[] { "VisitID" });
            DropIndex("dbo.Visitors", new[] { "ID" });
            DropTable("dbo.Visits");
            DropTable("dbo.Visitors");
        }
    }
}
