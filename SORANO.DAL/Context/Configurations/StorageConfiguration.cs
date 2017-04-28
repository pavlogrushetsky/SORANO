using SORANO.CORE.StockEntities;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Storage configuration
    /// </summary>
    internal class StorageConfiguration : StockEntityConfiguration<Storage>
    {
        /// <summary>
        /// Storage configuration
        /// </summary>
        public StorageConfiguration()
        {
            Property(s => s.ToDate)
                .IsOptional()
                .HasColumnType("datetime2");

            HasKey(s => s.GoodsID);

            HasKey(s => s.LocationID);

            HasKey(s => s.FromDate);

            Property(s => s.FromDate)
                .IsRequired()
                .HasColumnType("datetime2");

            ToTable("Storages");
        }
    }
}