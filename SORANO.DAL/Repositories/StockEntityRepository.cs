using SORANO.CORE;
using SORANO.DAL.Context;
using SORANO.DAL.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

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
        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().AsEnumerable();
        }

        /// <summary>
        /// Get all stock entities including specified properties
        /// </summary>
        /// <param name="properties">Properties to include in selection</param>
        /// <returns>Stock entities</returns>
        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] properties)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return query.AsEnumerable();
        }

        /// <summary>
        /// Get single stock entity
        /// </summary>
        /// <param name="id">Unique identifier of an entity</param>
        /// <returns>Stock entity</returns>
        public T Get(int id)
        {
            return _context.Set<T>().FirstOrDefault(x => x.ID == id);
        }

        /// <summary>
        /// Get single stock entity for which the specified predicate is true
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>Stock entity</returns>
        public T Get(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Get single stock entity for which the specified predicate is true including specified properties
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="properties">Properties to include in selection</param>
        /// <returns>Stock Entity</returns>
        public T Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] properties)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return query.Where(predicate).FirstOrDefault();
        }

        /// <summary>
        /// Find entities by specified predicate
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>Stock entities</returns>
        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        /// <summary>
        /// Get count of stock entities of specified type
        /// </summary>
        /// <returns>Count of entities</returns>
        public int Count()
        {
            return _context.Set<T>().Count();
        }

        /// <summary>
        /// Add stock entity
        /// </summary>
        /// <param name="entity">Stock entity to be added</param>
        public void Add(T entity)
        {
            var entry = _context.Entry(entity);
            _context.Set<T>().Add(entity);
        }

        /// <summary>
        /// Update stock entity
        /// </summary>
        /// <param name="entity">Stock entity to be updated</param>
        public void Update(T entity)
        {
            var entry = _context.Entry(entity);
            entry.State = EntityState.Modified;
        }

        /// <summary>
        /// Delete stock entity
        /// </summary>
        /// <param name="entity">Stock entity to be deleted</param>
        public void Delete(T entity)
        {
            var entry = _context.Entry(entity);
            entry.State = EntityState.Deleted;
        }

        /// <summary>
        /// Delete stock entities
        /// </summary>
        /// <param name="predicate">Predicate for entities to be deleted</param>
        public void Delete(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = _context.Set<T>().Where(predicate);

            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Deleted;
            }
        }

        /// <summary>
        /// Commit changes
        /// </summary>
        public void Commit()
        {
            _context.SaveChanges();
        }
    }
}
