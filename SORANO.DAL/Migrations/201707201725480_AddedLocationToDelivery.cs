namespace SORANO.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedLocationToDelivery : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Deliveries", "LocationID", c => c.Int(nullable: false));
            CreateIndex("dbo.Deliveries", "LocationID");
            AddForeignKey("dbo.Deliveries", "LocationID", "dbo.Locations", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Deliveries", "LocationID", "dbo.Locations");
            DropIndex("dbo.Deliveries", new[] { "LocationID" });
            DropColumn("dbo.Deliveries", "LocationID");
        }
    }
}
