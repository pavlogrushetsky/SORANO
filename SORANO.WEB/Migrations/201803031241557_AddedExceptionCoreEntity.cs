namespace SORANO.WEB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedExceptionCoreEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Exceptions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Message = c.String(nullable: false),
                        InnerException = c.String(nullable: false),
                        StackTrace = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Exceptions");
        }
    }
}
