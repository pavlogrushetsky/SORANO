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
    /// <summary>
    /// Generic repository for the stock entities
    /// </summary>
    /// <typeparam name="T">Stock entity type</typeparam>
    public class StockRepository<T> where T : Entity, new()
    {
        private readonly StockContext _context;
        private readonly IDbSet<T> _dataSet;

        /// <summary>
        /// Generic repository for the stock entities
        /// </summary>
        /// <param name="context"></param>
        public StockRepository(StockContext context)
        {
            _context = context;
            _dataSet = _context.Set<T>();
        }

        /// <summary>
        /// Get all stock entities
        /// </summary>
        /// <returns>Stock entities</returns>
        public virtual IEnumerable<T> GetAll()
        {
            return _dataSet.AsEnumerable();
        }

        /// <summary>
        /// Get all stock entities asyncronously
        /// </summary>
        /// <returns>Stock entities</returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var entities = await _dataSet.ToListAsync();

            return entities;
        }

        /// <summary>
        /// Get single stock entity
        /// </summary>
        /// <param name="id">Unique identifier of an entity</param>
        /// <returns>Stock entity</returns>
        public virtual T Get(int id)
        {
            return _dataSet.FirstOrDefault(x => x.ID == id);
        }

        /// <summary>
        /// Get single stock entity asyncronously
        /// </summary>
        /// <param name="id">Unique identifier of an entity</param>
        /// <returns>Stock entity</returns>
        public virtual async Task<T> GetAsync(int id)
        {
            return await _dataSet.FirstOrDefaultAsync(x => x.ID == id);
        }

        /// <summary>
        /// Get single stock entity for which the specified predicate is true
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>Stock entity</returns>
        public virtual T Get(Expression<Func<T, bool>> predicate)
        {
            return _dataSet.FirstOrDefault(predicate);
        }

        /// <summary>
        /// Get single stock entity asyncronously for which the specified predicate is true
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>Stock entity</returns>
        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dataSet.FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Find entities by specified predicate
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>Stock entities</returns>
        public virtual IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _dataSet.Where(predicate);
        }

        /// <summary>
        /// Find entities by specified predicate asynchronously
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>Stock entities</returns>
        public virtual async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dataSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Get count of stock entities of specified type
        /// </summary>
        /// <returns>Count of entities</returns>
        public virtual int Count()
        {
            return _dataSet.Count();
        }

        /// <summary>
        /// Get count of stock entities of specified type asynchronously
        /// </summary>
        /// <returns>Count of entities</returns>
        public virtual async Task<int> CountAsync()
        {
            return await _dataSet.CountAsync();
        }

        /// <summary>
        /// Add stock entity
        /// </summary>
        /// <param name="entity">Stock entity to be added</param>
        /// <returns>Added entity</returns> 
        public virtual T Add(T entity)
        {
            var addedEntity = _dataSet.Add(entity);

            return addedEntity;
        }

        /// <summary>
        /// Update stock entity
        /// </summary>
        /// <param name="entity">Stock entity to be updated</param>
        /// <returns>Updated entity</returns>   
        public virtual T Update(T entity)
        {
            var attachedEntity = _dataSet.Attach(entity);
            _context.Entry(attachedEntity).State = EntityState.Modified;

            return attachedEntity;
        }

        /// <summary>
        /// Delete stock entity
        /// </summary>
        /// <param name="entity">Stock entity to be deleted</param>
        public virtual void Delete(T entity)
        {
            _dataSet.Remove(entity);
        }

        /// <summary>
        /// Delete stock entities
        /// </summary>
        /// <param name="predicate">Predicate for entities to be deleted</param>
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
