namespace SORANO.WEB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRecommendedPriceToArticle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "RecommendedPrice", c => c.Decimal(precision: 38, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Articles", "RecommendedPrice");
        }
    }
}
