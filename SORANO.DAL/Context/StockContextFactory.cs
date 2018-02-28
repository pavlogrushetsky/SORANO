using System.Data.Entity.Infrastructure;

namespace SORANO.DAL.Context
{
    public class StockContextFactory : IDbContextFactory<StockContext>
    {
        public StockContext Create()
        {           
            return new StockContext("Server=PHRUSHETSKYI-LP;Database=SORANO_DEV;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}