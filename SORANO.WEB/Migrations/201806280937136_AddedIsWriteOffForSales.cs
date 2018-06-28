namespace SORANO.WEB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsWriteOffForSales : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "IsWriteOff", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "IsWriteOff");
        }
    }
}
