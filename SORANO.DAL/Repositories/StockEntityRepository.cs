using SORANO.CORE;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;
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
    public class StockEntityRepository<T> : IStockEntityRepository<T> where T : StockEntity, new()
    {
        // Data context
        private readonly StockContext _context;

        /// <summary>
        /// Generic repository for the stock entities
        /// </summary>
        /// <param name="context">Data context</param>
        public StockEntityRepository(StockContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all stock entities
        /// </summary>
        /// <returns>Stock entities</returns>
        public virtual IEnumerable<T> GetAll()
        {
            return _context.Set<T>().AsEnumerable();
        }

        /// <summary>
        /// Get all stock entities asyncronously
        /// </summary>
        /// <returns>Stock entities</returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        /// <summary>
        /// Get all stock entities including specified properties
        /// </summary>
        /// <param name="properties">Properties to include in selection</param>
        /// <returns>Stock entities</returns>
        public virtual IEnumerable<T> GetAll(params Expression<Func<T, object>>[] properties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = properties.Aggregate(query, (current, property) => current.Include(property));

            return query.AsEnumerable();
        }

        /// <summary>
        /// Get all stock entities including specified properties asyncronously
        /// </summary>
        /// <param name="properties">Properties to include in selection</param>
        /// <returns>Stock entities</returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] properties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = properties.Aggregate(query, (current, property) => current.Include(property));

            return await query.ToListAsync();
        }

        /// <summary>
        /// Get single stock entity
        /// </summary>
        /// <param name="id">Unique identifier of an entity</param>
        /// <returns>Stock entity</returns>
        public virtual T Get(int id)
        {
            return _context.Set<T>().FirstOrDefault(x => x.ID == id);
        }

        /// <summary>
        /// Get single stock entity asyncronously
        /// </summary>
        /// <param name="id">Unique identifier of an entity</param>
        /// <returns>Stock entity</returns>
        public virtual async Task<T> GetAsync(int id)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(x => x.ID == id);
        }

        /// <summary>
        /// Get single stock entity for which the specified predicate is true
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>Stock entity</returns>
        public virtual T Get(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Get single stock entity asyncronously for which the specified predicate is true
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>Stock entity</returns>
        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Get single stock entity for which the specified predicate is true including specified properties
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="properties">Properties to include in selection</param>
        /// <returns>Stock Entity</returns>
        public virtual T Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] properties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = properties.Aggregate(query, (current, property) => current.Include(property));

            return query.Where(predicate).FirstOrDefault();
        }

        /// <summary>
        /// Get single stock entity asynchronously for which the specified predicate is true including specified properties
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="properties">Properties to include in selection</param>
        /// <returns>Stock Entity</returns>
        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] properties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = properties.Aggregate(query, (current, property) => current.Include(property));

            return await query.Where(predicate).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Find entities by specified predicate
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>Stock entities</returns>
        public virtual IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        /// <summary>
        /// Find entities by specified predicate asynchronously
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>Stock entities</returns>
        public virtual async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Get count of stock entities of specified type
        /// </summary>
        /// <returns>Count of entities</returns>
        public virtual int Count()
        {
            return _context.Set<T>().Count();
        }

        /// <summary>
        /// Get count of stock entities of specified type asynchronously
        /// </summary>
        /// <returns>Count of entities</returns>
        public virtual async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        /// <summary>
        /// Add stock entity
        /// </summary>
        /// <param name="entity">Stock entity to be added</param>
        /// <returns>Added entity</returns> 
        public virtual T Add(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();

            return entity;
        }

        /// <summary>
        /// Add stock entity asynchronously
        /// </summary>
        /// <param name="entity">Stock entity to be added</param>
        /// <returns>Added entity</returns> 
        public virtual async Task<T> AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// Update stock entity
        /// </summary>
        /// <param name="entity">Stock entity to be updated</param>
        /// <returns>Updated entity</returns>   
        public virtual T Update(T entity)
        {
            var entry = _context.Entry(entity);
            entry.State = EntityState.Modified;
            _context.SaveChanges();

            return entity;
        }

        /// <summary>
        /// Update stock entity asynchronously
        /// </summary>
        /// <param name="entity">Stock entity to be updated</param>
        /// <returns>Updated entity</returns>   
        public virtual async Task<T> UpdateAsync(T entity)
        {
            var entry = _context.Entry(entity);
            entry.State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// Delete stock entity
        /// </summary>
        /// <param name="entity">Stock entity to be deleted</param>
        public virtual void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        /// <summary>
        /// Delete stock entity asynchronously
        /// </summary>
        /// <param name="entity">Stock entity to be deleted</param>
        public virtual async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete stock entities
        /// </summary>
        /// <param name="predicate">Predicate for entities to be deleted</param>
        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = _context.Set<T>().Where(predicate);

            foreach (var entity in entities)
            {
                _context.Set<T>().Remove(entity);
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// Delete stock entities asynchronously
        /// </summary>
        /// <param name="predicate">Predicate for entities to be deleted</param>
        public virtual async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = _context.Set<T>().Where(predicate);

            foreach (var entity in entities)
            {
                _context.Set<T>().Remove(entity);
            }

            await _context.SaveChangesAsync();
        }
    }
}
