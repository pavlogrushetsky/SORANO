// <auto-generated />
namespace SORANO.WEB.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.2.0-61023")]
    public sealed partial class AddedSaleEntities : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(AddedSaleEntities));
        
        string IMigrationMetadata.Id
        {
            get { return "201801271605253_AddedSaleEntities"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return "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"; }
        }
    }
}
