using SORANO.BLL.DTOs;
using SORANO.CORE.StockEntities;
using System.Linq;

namespace SORANO.BLL.Extensions
{
    internal static class DetailsDtoExtensions
    {
        public static DetailsDto FromModel(this StockEntity model)
        {
            return new DetailsDto
            {
                IsDeleted = model.IsDeleted,
                CanBeDeleted = !model.DeliveryItems.Any() && !model.IsDeleted,
                Created = model.CreatedDate,
                Modified = model.ModifiedDate,
                Deleted = model.DeletedDate,
                CreatedBy = model.CreatedByUser?.Login,
                ModifiedBy = model.ModifiedByUser?.Login,
                DeletedBy = model.DeletedByUser?.Login
            };
        }
    }
}
