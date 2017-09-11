using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class SupplierExtensions
    {
        public static SupplierDto ToDto(this Supplier model)
        {
            return new SupplierDto
            {

            };
        }

        public static Supplier ToEntity(this SupplierDto dto)
        {
            return new Supplier
            {

            };
        }

        public static void UpdateFields(this Supplier existentSupplier, Supplier newSupplier)
        {
            existentSupplier.Name = newSupplier.Name;
            existentSupplier.Description = newSupplier.Description;
        }
    }
}