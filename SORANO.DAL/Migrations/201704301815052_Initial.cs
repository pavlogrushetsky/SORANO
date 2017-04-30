namespace SORANO.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.StockEntities", new[] { "CreatedBy" });
            DropIndex("dbo.StockEntities", new[] { "ModifiedBy" });
            AlterColumn("dbo.StockEntities", "CreatedBy", c => c.Int());
            AlterColumn("dbo.StockEntities", "ModifiedBy", c => c.Int());
            CreateIndex("dbo.StockEntities", "CreatedBy");
            CreateIndex("dbo.StockEntities", "ModifiedBy");
        }
        
        public override void Down()
        {
            DropIndex("dbo.StockEntities", new[] { "ModifiedBy" });
            DropIndex("dbo.StockEntities", new[] { "CreatedBy" });
            AlterColumn("dbo.StockEntities", "ModifiedBy", c => c.Int(nullable: false));
            AlterColumn("dbo.StockEntities", "CreatedBy", c => c.Int(nullable: false));
            CreateIndex("dbo.StockEntities", "ModifiedBy");
            CreateIndex("dbo.StockEntities", "CreatedBy");
        }
    }
}
