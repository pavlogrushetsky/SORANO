// <auto-generated />
namespace SORANO.WEB.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.2.0-61023")]
    public sealed partial class OneToManyForSaleGoods : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(OneToManyForSaleGoods));
        
        string IMigrationMetadata.Id
        {
            get { return "201801291919002_OneToManyForSaleGoods"; }
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
