using System.Data.Common;
using System.Data.Entity;
using SORANO.CORE;
using SORANO.CORE.AccountEntities;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Context.Configurations;
using SORANO.DAL.Migrations;
using System.Threading.Tasks;

namespace SORANO.DAL.Context
{
    /// <summary>
    /// Data context
    /// </summary>
    public class StockContext : DbContext
    {
        /// <summary>
        /// Data context
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        public StockContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<StockContext, Configuration>(true));
        }

        /// <summary>
        /// Data context with default connection string
        /// </summary>
        public StockContext() : base("SORANO")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<StockContext, Configuration>("SORANO"));
        }

        /// <summary>
        /// Stock entities
        /// </summary>
        public IDbSet<StockEntity> StockEntities { get; set; }

        /// <summary>
        /// Articles
        /// </summary>
        public IDbSet<Article> Articles { get; set; }

        /// <summary>
        /// Article types
        /// </summary>
        public IDbSet<ArticleType> ArticleTypes { get; set; }

        /// <summary>
        /// Attachments
        /// </summary>
        public IDbSet<Attachment> Attachments { get; set; }

        /// <summary>
        /// Attachment types
        /// </summary>
        public IDbSet<AttachmentType> AttachmentTypes { get; set; }

        /// <summary>
        /// Clients
        /// </summary>
        public IDbSet<Client> Clients { get; set; }

        /// <summary>
        /// Deliveries
        /// </summary>
        public IDbSet<Delivery> Deliveries { get; set; }

        /// <summary>
        /// Delivery items
        /// </summary>
        public IDbSet<DeliveryItem> DeliveryItems { get; set; }

        /// <summary>
        /// Goods
        /// </summary>
        public IDbSet<Goods> Goods { get; set; }

        /// <summary>
        /// Locations
        /// </summary>
        public IDbSet<Location> Locations { get; set; }

        /// <summary>
        /// Location types
        /// </summary>
        public IDbSet<LocationType> LocationTypes { get; set; }

        /// <summary>
        /// Recommendations
        /// </summary>
        public IDbSet<Recommendation> Recommendations { get; set; }

        /// <summary>
        /// Storages
        /// </summary>
        public IDbSet<Storage> Storages { get; set; }

        /// <summary>
        /// Suppliers
        /// </summary>
        public IDbSet<Supplier> Suppliers { get; set; }

        /// <summary>
        /// Users
        /// </summary>
        public IDbSet<User> Users { get; set; }

        /// <summary>
        /// Roles
        /// </summary>
        public IDbSet<Role> Roles { get; set; }

        /// <summary>
        /// Commit changes asynchronously
        /// </summary>
        /// <returns></returns>
        public virtual async Task CommitAsync()
        {
            await base.SaveChangesAsync();
        }

        /// <summary>
        /// Commit changes
        /// </summary>
        public virtual void Commit()
        {
            base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            // Adding configuration for stock entities
            builder.Configurations.Add(new StockEntityConfiguration());
            builder.Configurations.Add(new UserConfiguration());
            builder.Configurations.Add(new RoleConfiguration());
            builder.Configurations.Add(new ArticleConfiguration());
            builder.Configurations.Add(new ArticleTypeConfiguration());
            builder.Configurations.Add(new AttachmentConfiguration());
            builder.Configurations.Add(new AttachmentTypeConfiguration());
            builder.Configurations.Add(new ClientConfiguration());
            builder.Configurations.Add(new DeliveryConfiguration());
            builder.Configurations.Add(new DeliveryItemConfiguration());
            builder.Configurations.Add(new GoodsConfiguration());
            builder.Configurations.Add(new LocationConfiguration());
            builder.Configurations.Add(new LocationTypeConfiguration());
            builder.Configurations.Add(new RecommendationConfiguration());
            builder.Configurations.Add(new StorageConfiguration());
            builder.Configurations.Add(new SupplierConfiguration());
        }
    }
}
