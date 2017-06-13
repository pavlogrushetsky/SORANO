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
                Name = supplier.Name,
                Description = supplier.Description,
                Recommendations = supplier.Recommendations?.Select(r => r.ToModel()).ToList()
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
