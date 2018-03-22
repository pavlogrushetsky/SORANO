namespace SORANO.WEB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTimestampForException : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Exceptions", "Timestamp", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Exceptions", "Timestamp");
        }
    }
}
