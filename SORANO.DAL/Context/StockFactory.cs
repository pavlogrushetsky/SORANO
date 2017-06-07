using System;

namespace SORANO.DAL.Context
{
    public class StockFactory : Disposable, IStockFactory
    {
        private StockContext _context;

        public StockContext Init()
        {
            return _context ?? (_context = new StockContext("SORANO"));
        }

        protected override void DisposeCore()
        {
            _context?.Dispose();
        }
    }

    public class Disposable : IDisposable
    {
        private bool _isDisposed;

        ~Disposable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed && disposing)
            {
                DisposeCore();
            }

            _isDisposed = true;
        }

        protected virtual void DisposeCore()
        {
        }
    }
}
