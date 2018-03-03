namespace SORANO.WEB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsCachlessFalgForSale : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "IsCachless", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "IsCachless");
        }
    }
}
