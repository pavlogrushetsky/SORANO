namespace SORANO.DAL.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddedExtensionsToAttachmentType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AttachmentTypes", "Extensions", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AttachmentTypes", "Extensions");
        }
    }
}
