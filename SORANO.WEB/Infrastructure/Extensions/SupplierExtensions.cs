using SORANO.CORE.StockEntities;
using System.Linq;
using SORANO.WEB.Models;

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
                Recommendations = supplier.Recommendations?.Where(r => !r.IsDeleted).Select(r => r.ToModel()).ToList(),
                MainPicture = supplier.Attachments?.SingleOrDefault(a => !a.IsDeleted && a.Type.Name.Equals("Основное изображение"))?.ToModel() ?? new AttachmentModel(),
                Attachments = supplier.Attachments?.Where(a => !a.IsDeleted && !a.Type.Name.Equals("Основное изображение")).Select(a => a.ToModel()).ToList(),
                CanBeDeleted = !supplier.Deliveries.Any() && !supplier.IsDeleted,
                IsDeleted = supplier.IsDeleted,
                Created = supplier.CreatedDate.ToString("dd.MM.yyyy"),
                Modified = supplier.ModifiedDate.ToString("dd.MM.yyyy"),
                Deleted = supplier.DeletedDate?.ToString("dd.MM.yyyy"),
                CreatedBy = supplier.CreatedByUser?.Login,
                ModifiedBy = supplier.ModifiedByUser?.Login,
                DeletedBy = supplier.DeletedByUser?.Login
            };
        }

        public static Supplier ToEntity(this SupplierModel model)
        {
            var supplier = new Supplier
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                Recommendations = model.Recommendations.Select(r => r.ToEntity()).ToList(),
                Attachments = model.Attachments.Select(a => a.ToEntity()).ToList()
            };

            if (!string.IsNullOrEmpty(model.MainPicture?.Name))
            {
                supplier.Attachments.Add(model.MainPicture.ToEntity());
            }

            return supplier;
        }
    }
}
