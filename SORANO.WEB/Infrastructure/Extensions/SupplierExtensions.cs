using SORANO.CORE.StockEntities;
using SORANO.WEB.Models.Supplier;
using System.Linq;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class SupplierExtensions
    {
        public static SupplierModel ToModel(this Supplier supplier)
        {
            return new SupplierModel
            {
                ID = supplier.ID,
                Name = supplier.Name,
                Description = supplier.Description,
                Recommendations = supplier.Recommendations?.Select(r => r.ToModel()).ToList(),
                CanBeDeleted = !supplier.Deliveries.Any(),
                Created = supplier.CreatedDate.ToString("dd.MM.yyyy"),
                Modified = supplier.ModifiedDate.ToString("dd.MM.yyyy"),
                CreatedBy = supplier.CreatedByUser?.Login,
                ModifiedBy = supplier.ModifiedByUser?.Login
            };
        }

        public static Supplier ToEntity(this SupplierModel model)
        {
            return new Supplier
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                Recommendations = model.Recommendations.Select(r => r.ToEntity()).ToList()
            };
        }
    }
}
