using System.Threading.Tasks;

namespace SORANO.DAL.Context
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IStockFactory _factory;
        private StockContext _context;

        public UnitOfWork(IStockFactory factory)
        {
            _factory = factory;
        }

        public StockContext Context
        {
            get { return _context ?? (_context = _factory.Init()); }
        }

        public void Commit()
        {
            Context.Commit();
        }

        public async Task CommitAsync()
        {
            await Context.CommitAsync();
        }
    }
}
