using SORANO.CORE.StockEntities;
using SORANO.WEB.ViewModels;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class StockEntityExtensions
    {
        public static UserActivityModel ToUserActivityModel(this StockEntity entity, string type)
        {
            return new UserActivityModel
            {
                ActivityType = type,
                EntityID = entity.ID,
                EntityType = entity.GetType().Name,
                Date = entity.CreatedDate.ToString("dd.MM.yyyy")
            };
        }
    }
}