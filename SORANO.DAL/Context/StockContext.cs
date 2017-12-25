using System.Data.Entity;
using SORANO.CORE.AccountEntities;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Context.Configurations;
using SORANO.DAL.Migrations;
using System.Threading.Tasks;

namespace SORANO.DAL.Context
{
    public class StockContext : DbContext
    {
        public StockContext(string connectionString) : base(connectionString)
        {

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<StockContext, Configuration>(true));

            Database.Initialize(true);
        }

        public StockContext() : base("SORANO")
        {

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<StockContext, Configuration>("SORANO"));

            Database.Initialize(true);
        }

        public IDbSet<StockEntity> StockEntities { get; set; }

        public IDbSet<Article> Articles { get; set; }

        public IDbSet<ArticleType> ArticleTypes { get; set; }

        public IDbSet<Attachment> Attachments { get; set; }

        public IDbSet<AttachmentType> AttachmentTypes { get; set; }

        public IDbSet<Client> Clients { get; set; }

        public IDbSet<Delivery> Deliveries { get; set; }

        public IDbSet<DeliveryItem> DeliveryItems { get; set; }

        public IDbSet<Goods> Goods { get; set; }

        public IDbSet<Location> Locations { get; set; }

        public IDbSet<LocationType> LocationTypes { get; set; }

        public IDbSet<Recommendation> Recommendations { get; set; }

        public IDbSet<Storage> Storages { get; set; }

        public IDbSet<Supplier> Suppliers { get; set; }

        public IDbSet<User> Users { get; set; }

        public IDbSet<Role> Roles { get; set; }

        public IDbSet<Sale> Sales { get; set; }

        public IDbSet<SaleItem> SaleItems { get; set; }

        public virtual async Task CommitAsync()
        {
            await SaveChangesAsync();
        }

        public virtual void Commit()
        {
            SaveChanges();
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
            builder.Configurations.Add(new SaleConfiguration());
            builder.Configurations.Add(new SaleItemConfiguration());
        }
    }
}
