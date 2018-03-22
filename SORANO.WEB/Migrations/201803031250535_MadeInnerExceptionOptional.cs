namespace SORANO.WEB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeInnerExceptionOptional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Exceptions", "InnerException", c => c.String());
            AlterColumn("dbo.Exceptions", "StackTrace", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Exceptions", "StackTrace", c => c.String());
            AlterColumn("dbo.Exceptions", "InnerException", c => c.String(nullable: false));
        }
    }
}
