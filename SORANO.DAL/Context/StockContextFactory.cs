using System.Data.Entity.Infrastructure;

namespace SORANO.DAL.Context
{
    public class StockContextFactory : IDbContextFactory<StockContext>
    {
        public StockContext Create()
        {           
            return new StockContext("Data Source=GRUSHETSKY-PC;Initial Catalog=SORANO_DEV;Integrated Security=True");
        }
    }
}