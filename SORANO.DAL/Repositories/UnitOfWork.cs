using SORANO.CORE;
using SORANO.DAL.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Unit of work
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StockContext _context;
        private readonly IDictionary<string, object> _repositories;

        /// <summary>
        /// Unit of work
        /// </summary>
        /// <param name="context">Data context</param>
        public UnitOfWork(StockContext context)
        {
            _context = context;
            _repositories = new Dictionary<string, object>();
        }

        /// <summary>
        /// Get repository
        /// </summary>
        /// <typeparam name="T">Type parameter</typeparam>
        /// <returns>Repository of specified type</returns>
        public StockRepository<T> Get<T>() where T : Entity, new()
        {
            var type = typeof(T);
            var key = type.ToString();

            if (_repositories.ContainsKey(key))
            {
                return _repositories[key] as StockRepository<T>;
            }
            else
            {
                var repo = new StockRepository<T>(_context);
                _repositories.Add(key, repo);

                return repo;
            }
        }

        /// <summary>
        /// Save changes
        /// </summary>
        public void Save()
        {
            _context.Commit();
        }

        /// <summary>
        /// Save changes asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await _context.CommitAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
