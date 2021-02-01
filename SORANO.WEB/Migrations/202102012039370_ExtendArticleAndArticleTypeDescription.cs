namespace SORANO.WEB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendArticleAndArticleTypeDescription : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Articles", "Name", c => c.String(nullable: false, maxLength: 1500));
            AlterColumn("dbo.Articles", "Description", c => c.String(maxLength: 3000));
            AlterColumn("dbo.Articles", "Producer", c => c.String(maxLength: 600));
            AlterColumn("dbo.ArticleTypes", "Name", c => c.String(nullable: false, maxLength: 1500));
            AlterColumn("dbo.ArticleTypes", "Description", c => c.String(maxLength: 3000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ArticleTypes", "Description", c => c.String(maxLength: 1000));
            AlterColumn("dbo.ArticleTypes", "Name", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.Articles", "Producer", c => c.String(maxLength: 200));
            AlterColumn("dbo.Articles", "Description", c => c.String(maxLength: 1000));
            AlterColumn("dbo.Articles", "Name", c => c.String(nullable: false, maxLength: 500));
        }
    }
}
