using System;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Helpers
{
    internal static class StockEntityHelper
    {
        public static StockEntity UpdateCreatedFields(this StockEntity entity, int userId)
        {
            entity.CreatedBy = userId;
            entity.CreatedDate = DateTime.Now;

            return entity;
        }

        public static StockEntity UpdateModifiedFields(this StockEntity entity, int userId)
        {
            entity.ModifiedBy = userId;
            entity.ModifiedDate = DateTime.Now;

            return entity;
        }

        public static StockEntity UpdateDeletedFields(this StockEntity entity, int userId)
        {
            entity.DeletedBy = userId;
            entity.DeletedDate = DateTime.Now;
            entity.IsDeleted = true;

            return entity;
        }
    }
}