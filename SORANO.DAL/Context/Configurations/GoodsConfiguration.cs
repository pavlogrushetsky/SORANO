using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    internal class GoodsConfiguration : StockEntityConfiguration<Goods>
    {
        public GoodsConfiguration()
        {           
            HasMany(g => g.Storages)
                .WithRequired(s => s.Goods)
                .HasForeignKey(s => s.GoodsID);

            Property(d => d.Price)
                .IsOptional()
                .HasPrecision(38, 2);

            ToTable("Goods");
        }
    }
}