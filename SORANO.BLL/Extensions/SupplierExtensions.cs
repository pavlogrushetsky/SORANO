using System.Linq;
using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class SupplierExtensions
    {
        public static SupplierDto ToDto(this Supplier model)
        {
            var dto = new SupplierDto
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                Deliveries = model.Deliveries.Where(d => !d.IsDeleted).Select(d => d.ToDto())
            };

            dto.MapDetails(model);
            dto.CanBeDeleted = (!model.Deliveries?.Any() ?? false) && !model.IsDeleted;

            return dto;
        }

        public static Supplier ToEntity(this SupplierDto dto)
        {
            var entity = new Supplier
            {
                ID = dto.ID,
                Name = dto.Name,
                Description = dto.Description,
                Recommendations = dto.Recommendations.Select(r => r.ToEntity()).ToList(),
                Attachments = dto.Attachments.Select(a => a.ToEntity()).ToList()
            };

            if (!string.IsNullOrEmpty(dto.MainPicture?.FullPath))
                entity.Attachments.Add(dto.MainPicture.ToEntity());

            return entity;
        }

        public static void UpdateFields(this Supplier existentSupplier, Supplier newSupplier)
        {
            existentSupplier.Name = newSupplier.Name;
            existentSupplier.Description = newSupplier.Description;
        }
    }
}