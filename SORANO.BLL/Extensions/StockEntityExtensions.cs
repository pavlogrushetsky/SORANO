using System;
using System.Data.Objects;
using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class StockEntityExtensions
    {
        public static UserActivityDto ToUserActivityDto(this StockEntity entity, UserActivityType activityType)
        {
            var dto = new UserActivityDto
            {
                EntityID = entity.ID,
                Type = activityType,
                EntityName = ObjectContext.GetObjectType(entity.GetType()).BaseType?.Name
            };

            switch (activityType)
            {
                case UserActivityType.Creation:
                    dto.DateTime = entity.CreatedDate;
                    break;
                case UserActivityType.Updating:
                    dto.DateTime = entity.ModifiedDate;
                    break;
                case UserActivityType.Deletion:
                    if (entity.DeletedDate != null)
                        dto.DateTime = entity.DeletedDate.Value;
                    break;
                default:
                    throw new ArgumentException(nameof(activityType));
            }

            return dto;
        }
    }
}