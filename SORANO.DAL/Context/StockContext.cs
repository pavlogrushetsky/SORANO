using System.Data.Entity;

namespace SORANO.DAL.Context
{
    public class StockContext : DbContext
    {
        public StockContext(string connectionString) : base(connectionString)
        {

        }
    }
}
