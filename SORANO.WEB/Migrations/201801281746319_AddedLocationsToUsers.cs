namespace SORANO.WEB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedLocationsToUsers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UsersLocations",
                c => new
                    {
                        UserID = c.Int(nullable: false),
                        LocationID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserID, t.LocationID })
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .ForeignKey("dbo.Locations", t => t.LocationID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.LocationID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersLocations", "LocationID", "dbo.Locations");
            DropForeignKey("dbo.UsersLocations", "UserID", "dbo.Users");
            DropIndex("dbo.UsersLocations", new[] { "LocationID" });
            DropIndex("dbo.UsersLocations", new[] { "UserID" });
            DropTable("dbo.UsersLocations");
        }
    }
}
