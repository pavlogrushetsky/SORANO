using SORANO.CORE;
using SORANO.DAL.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StockContext _context;
        private readonly IDictionary<string, object> _repositories;

        public UnitOfWork(StockContext context)
        {
            _context = context;
            _repositories = new Dictionary<string, object>();
        }

        public StockRepository<T> Get<T>() where T : Entity, new()
        {
            var type = typeof(T);
            var key = type.ToString();

            if (_repositories.ContainsKey(key))
            {
                return _repositories[key] as StockRepository<T>;
            }

            var repo = new StockRepository<T>(_context);
            _repositories.Add(key, repo);

            return repo;
        }

        public void Save()
        {
            _context.Commit();
        }

        public async Task SaveAsync()
        {
            await _context.CommitAsync();
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
