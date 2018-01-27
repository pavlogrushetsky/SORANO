using System.Data.Entity.Migrations;

namespace SORANO.WEB.Migrations
{
    public partial class AddedIsDeletedFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockEntities", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Roles", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Roles", "IsDeleted");
            DropColumn("dbo.Users", "IsDeleted");
            DropColumn("dbo.StockEntities", "IsDeleted");
        }
    }
}
