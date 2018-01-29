using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;
using System.Linq;

namespace SORANO.BLL.Extensions
{
    internal static class SaleExtensions
    {
        public static SaleDto ToDto(this Sale model)
        {
            var dto = new SaleDto
            {
                ID = model.ID,
                ClientID = model.ClientID,
                Client = model.Client?.ToDto(),
                LocationID = model.LocationID,
                Location = model.Location.ToDto(),
                UserID = model.UserID,
                User = model.User.ToDto(),
                IsSubmitted = model.IsSubmitted,
                Date = model.Date,
                Items = model.Items.Where(di => !di.IsDeleted).Select(i => i.ToDto())
            };

            dto.MapDetails(model);
            dto.CanBeDeleted = !(model.IsSubmitted || model.IsDeleted);

            return dto;
        }

        public static Sale ToEntity(this SaleDto dto)
        {
            var entity = new Sale
            {

            };

            return entity;
        }

        public static void UpdateFields(this Sale existentSale, Sale newSale)
        {

        }
    }
}
