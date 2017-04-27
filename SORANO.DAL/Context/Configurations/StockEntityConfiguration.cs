using System.Data.Entity.ModelConfiguration;
using SORANO.CORE;

namespace SORANO.DAL.Context.Configurations
{
    /// <summary>
    /// Stock entity configuration
    /// </summary>
    /// <typeparam name="T">Stock entity type</typeparam>
    internal abstract class StockEntityConfiguration<T> : EntityTypeConfiguration<T> where T : StockEntity
    {
        /// <summary>
        /// Stock entity configuration
        /// </summary>
        protected StockEntityConfiguration()
        {
            HasKey(e => e.ID);
        }
    }
}