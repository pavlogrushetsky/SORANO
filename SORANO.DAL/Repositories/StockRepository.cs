using SORANO.CORE;
using SORANO.DAL.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SORANO.DAL.Repositories
{
    public class StockRepository<T> : IStockRepository<T> where T : Entity, new()
    {
        private readonly StockContext _context;
        private readonly IDbSet<T> _dataSet;

        public StockRepository(StockContext context)
        {
            _context = context;
            _dataSet = _context.Set<T>();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dataSet.AsEnumerable();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var entities = await _dataSet.ToListAsync();

            return entities;
        }

        public virtual T Get(int id)
        {
            return _dataSet.FirstOrDefault(x => x.ID == id);
        }

        public virtual async Task<T> GetAsync(int id)
        {
            return await _dataSet.FirstOrDefaultAsync(x => x.ID == id);
        }

        public virtual T Get(Expression<Func<T, bool>> predicate)
        {
            return _dataSet.FirstOrDefault(predicate);
        }

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dataSet.FirstOrDefaultAsync(predicate);
        }

        public virtual IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _dataSet.Where(predicate);
        }

        public virtual async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dataSet.Where(predicate).ToListAsync();
        }

        public virtual int Count()
        {
            return _dataSet.Count();
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dataSet.CountAsync();
        }

        public virtual T Add(T entity)
        {
            var addedEntity = _dataSet.Add(entity);

            return addedEntity;
        }

        public virtual T Update(T entity)
        {
            var attachedEntity = _dataSet.Attach(entity);
            _context.Entry(attachedEntity).State = EntityState.Modified;

            return attachedEntity;
        }

        public virtual void Delete(T entity)
        {
            _dataSet.Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = _dataSet.Where(predicate);

            foreach (var entity in entities)
            {
                _dataSet.Remove(entity);
            }
        }
    }
}
