namespace SORANO.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockEntities",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ModifiedBy = c.Int(nullable: false),
                        DeletedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        DeletedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.EntitiesAttachments",
                c => new
                    {
                        AttachmentID = c.Int(nullable: false),
                        EntityID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AttachmentID, t.EntityID })
                .ForeignKey("dbo.StockEntities", t => t.AttachmentID)
                .ForeignKey("dbo.Attachments", t => t.EntityID)
                .Index(t => t.AttachmentID)
                .Index(t => t.EntityID);
            
            CreateTable(
                "dbo.UsersRoles",
                c => new
                    {
                        UserID = c.Int(nullable: false),
                        RoleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserID, t.RoleID })
                .ForeignKey("dbo.Users", t => t.UserID)
                .ForeignKey("dbo.Roles", t => t.RoleID)
                .Index(t => t.UserID)
                .Index(t => t.RoleID);
            
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        TypeID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 200),
                        Description = c.String(maxLength: 1000),
                        Producer = c.String(nullable: false, maxLength: 200),
                        Code = c.String(nullable: false, maxLength: 50),
                        Barcode = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .ForeignKey("dbo.ArticleTypes", t => t.TypeID)
                .Index(t => t.ID)
                .Index(t => t.TypeID);
            
            CreateTable(
                "dbo.ArticleTypes",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        ParentTypeId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 200),
                        Description = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .ForeignKey("dbo.ArticleTypes", t => t.ParentTypeId)
                .Index(t => t.ID)
                .Index(t => t.ParentTypeId);
            
            CreateTable(
                "dbo.Attachments",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        AttachmentTypeID = c.Int(nullable: false),
                        FullPath = c.String(nullable: false, maxLength: 500),
                        Name = c.String(nullable: false, maxLength: 200),
                        Description = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .ForeignKey("dbo.AttachmentTypes", t => t.AttachmentTypeID)
                .Index(t => t.ID)
                .Index(t => t.AttachmentTypeID);
            
            CreateTable(
                "dbo.AttachmentTypes",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Comment = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 200),
                        Description = c.String(maxLength: 500),
                        PhoneNumber = c.String(maxLength: 50),
                        CardNumber = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.Deliveries",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        SupplierID = c.Int(nullable: false),
                        BillNumber = c.String(nullable: false, maxLength: 50),
                        DeliveryDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        PaymentDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        DollarRate = c.Decimal(precision: 38, scale: 2),
                        EuroRate = c.Decimal(precision: 38, scale: 2),
                        TotalGrossPrice = c.Decimal(nullable: false, precision: 38, scale: 2),
                        TotalDiscount = c.Decimal(nullable: false, precision: 38, scale: 2),
                        TotalDiscountedPrice = c.Decimal(nullable: false, precision: 38, scale: 2),
                        IsSubmitted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID)
                .Index(t => t.ID)
                .Index(t => t.SupplierID);
            
            CreateTable(
                "dbo.DeliveryItems",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        DeliveryID = c.Int(nullable: false),
                        ArticleID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 38, scale: 2),
                        GrossPrice = c.Decimal(nullable: false, precision: 38, scale: 2),
                        Discount = c.Decimal(nullable: false, precision: 38, scale: 2),
                        DiscountedPrice = c.Decimal(nullable: false, precision: 38, scale: 2),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .ForeignKey("dbo.Deliveries", t => t.DeliveryID)
                .ForeignKey("dbo.Articles", t => t.ArticleID)
                .Index(t => t.ID)
                .Index(t => t.DeliveryID)
                .Index(t => t.ArticleID);
            
            CreateTable(
                "dbo.Goods",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        DeliveryItemID = c.Int(nullable: false),
                        ClientID = c.Int(),
                        Marker = c.String(nullable: false, maxLength: 100),
                        SalePrice = c.Decimal(precision: 38, scale: 2),
                        SaleDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        SoldBy = c.Int(),
                        SaleLocationID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .ForeignKey("dbo.DeliveryItems", t => t.DeliveryItemID)
                .ForeignKey("dbo.Clients", t => t.ClientID)
                .ForeignKey("dbo.Users", t => t.SoldBy)
                .ForeignKey("dbo.Locations", t => t.SaleLocationID)
                .Index(t => t.ID)
                .Index(t => t.DeliveryItemID)
                .Index(t => t.ClientID)
                .Index(t => t.SoldBy)
                .Index(t => t.SaleLocationID);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        TypeID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 200),
                        Comment = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .ForeignKey("dbo.LocationTypes", t => t.TypeID)
                .Index(t => t.ID)
                .Index(t => t.TypeID);
            
            CreateTable(
                "dbo.LocationTypes",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.Recommendations",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        ParentEntityID = c.Int(nullable: false),
                        Value = c.Decimal(precision: 38, scale: 2),
                        Comment = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ParentEntityID)
                .Index(t => t.ID)
                .Index(t => t.ParentEntityID);
            
            CreateTable(
                "dbo.Storages",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        GoodsID = c.Int(nullable: false),
                        LocationID = c.Int(nullable: false),
                        FromDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        ToDate = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .ForeignKey("dbo.Goods", t => t.GoodsID)
                .ForeignKey("dbo.Locations", t => t.LocationID)
                .Index(t => t.ID)
                .Index(t => new { t.GoodsID, t.LocationID, t.FromDate }, unique: true, name: "IX_Storage");
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 200),
                        Description = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Email = c.String(),
                        Password = c.String(),
                        IsBlocked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StockEntities", t => t.ID)
                .Index(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Roles", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.Users", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.Suppliers", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.Storages", "LocationID", "dbo.Locations");
            DropForeignKey("dbo.Storages", "GoodsID", "dbo.Goods");
            DropForeignKey("dbo.Storages", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.Recommendations", "ParentEntityID", "dbo.StockEntities");
            DropForeignKey("dbo.Recommendations", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.LocationTypes", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.Locations", "TypeID", "dbo.LocationTypes");
            DropForeignKey("dbo.Locations", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.Goods", "SaleLocationID", "dbo.Locations");
            DropForeignKey("dbo.Goods", "SoldBy", "dbo.Users");
            DropForeignKey("dbo.Goods", "ClientID", "dbo.Clients");
            DropForeignKey("dbo.Goods", "DeliveryItemID", "dbo.DeliveryItems");
            DropForeignKey("dbo.Goods", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.DeliveryItems", "ArticleID", "dbo.Articles");
            DropForeignKey("dbo.DeliveryItems", "DeliveryID", "dbo.Deliveries");
            DropForeignKey("dbo.DeliveryItems", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.Deliveries", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.Deliveries", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.Clients", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.AttachmentTypes", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.Attachments", "AttachmentTypeID", "dbo.AttachmentTypes");
            DropForeignKey("dbo.Attachments", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.ArticleTypes", "ParentTypeId", "dbo.ArticleTypes");
            DropForeignKey("dbo.ArticleTypes", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.Articles", "TypeID", "dbo.ArticleTypes");
            DropForeignKey("dbo.Articles", "ID", "dbo.StockEntities");
            DropForeignKey("dbo.UsersRoles", "RoleID", "dbo.Roles");
            DropForeignKey("dbo.UsersRoles", "UserID", "dbo.Users");
            DropForeignKey("dbo.StockEntities", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.StockEntities", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.StockEntities", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.EntitiesAttachments", "EntityID", "dbo.Attachments");
            DropForeignKey("dbo.EntitiesAttachments", "AttachmentID", "dbo.StockEntities");
            DropIndex("dbo.Roles", new[] { "ID" });
            DropIndex("dbo.Users", new[] { "ID" });
            DropIndex("dbo.Suppliers", new[] { "ID" });
            DropIndex("dbo.Storages", "IX_Storage");
            DropIndex("dbo.Storages", new[] { "ID" });
            DropIndex("dbo.Recommendations", new[] { "ParentEntityID" });
            DropIndex("dbo.Recommendations", new[] { "ID" });
            DropIndex("dbo.LocationTypes", new[] { "ID" });
            DropIndex("dbo.Locations", new[] { "TypeID" });
            DropIndex("dbo.Locations", new[] { "ID" });
            DropIndex("dbo.Goods", new[] { "SaleLocationID" });
            DropIndex("dbo.Goods", new[] { "SoldBy" });
            DropIndex("dbo.Goods", new[] { "ClientID" });
            DropIndex("dbo.Goods", new[] { "DeliveryItemID" });
            DropIndex("dbo.Goods", new[] { "ID" });
            DropIndex("dbo.DeliveryItems", new[] { "ArticleID" });
            DropIndex("dbo.DeliveryItems", new[] { "DeliveryID" });
            DropIndex("dbo.DeliveryItems", new[] { "ID" });
            DropIndex("dbo.Deliveries", new[] { "SupplierID" });
            DropIndex("dbo.Deliveries", new[] { "ID" });
            DropIndex("dbo.Clients", new[] { "ID" });
            DropIndex("dbo.AttachmentTypes", new[] { "ID" });
            DropIndex("dbo.Attachments", new[] { "AttachmentTypeID" });
            DropIndex("dbo.Attachments", new[] { "ID" });
            DropIndex("dbo.ArticleTypes", new[] { "ParentTypeId" });
            DropIndex("dbo.ArticleTypes", new[] { "ID" });
            DropIndex("dbo.Articles", new[] { "TypeID" });
            DropIndex("dbo.Articles", new[] { "ID" });
            DropIndex("dbo.UsersRoles", new[] { "RoleID" });
            DropIndex("dbo.UsersRoles", new[] { "UserID" });
            DropIndex("dbo.EntitiesAttachments", new[] { "EntityID" });
            DropIndex("dbo.EntitiesAttachments", new[] { "AttachmentID" });
            DropIndex("dbo.StockEntities", new[] { "DeletedBy" });
            DropIndex("dbo.StockEntities", new[] { "ModifiedBy" });
            DropIndex("dbo.StockEntities", new[] { "CreatedBy" });
            DropTable("dbo.Roles");
            DropTable("dbo.Users");
            DropTable("dbo.Suppliers");
            DropTable("dbo.Storages");
            DropTable("dbo.Recommendations");
            DropTable("dbo.LocationTypes");
            DropTable("dbo.Locations");
            DropTable("dbo.Goods");
            DropTable("dbo.DeliveryItems");
            DropTable("dbo.Deliveries");
            DropTable("dbo.Clients");
            DropTable("dbo.AttachmentTypes");
            DropTable("dbo.Attachments");
            DropTable("dbo.ArticleTypes");
            DropTable("dbo.Articles");
            DropTable("dbo.UsersRoles");
            DropTable("dbo.EntitiesAttachments");
            DropTable("dbo.StockEntities");
        }
    }
}
