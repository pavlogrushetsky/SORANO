using SORANO.CORE;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SORANO.DAL.Repositories.Abstract
{
    /// <summary>
    /// Generic abstract repository for the stock entities
    /// </summary>
    /// <typeparam name="T">Stock entity type</typeparam>
    public interface IStockEntityRepository<T> where T : StockEntity, new()
    {
        /// <summary>
        /// Get all stock entities
        /// </summary>
        /// <returns>Stock entities</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Get all stock entities including specified properties
        /// </summary>
        /// <param name="properties">Properties to include in selection</param>
        /// <returns>Stock entities</returns>
        IEnumerable<T> GetAll(params Expression<Func<T, object>>[] properties);

        /// <summary>
        /// Get single stock entity
        /// </summary>
        /// <param name="id">Unique identifier of an entity</param>
        /// <returns>Stock entity</returns>
        T Get(int id);

        /// <summary>
        /// Get single stock entity for which the specified predicate is true
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>Stock entity</returns>
        T Get(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Get single stock entity for which the specified predicate is true including specified properties
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="properties">Properties to include in selection</param>
        /// <returns>Stock Entity</returns>
        T Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] properties);

        /// <summary>
        /// Find entities by specified predicate
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>Stock entities</returns>
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Get count of stock entities of specified type
        /// </summary>
        /// <returns>Count of entities</returns>
        int Count();

        /// <summary>
        /// Add stock entity
        /// </summary>
        /// <param name="entity">Stock entity to be added</param>
        void Add(T entity);

        /// <summary>
        /// Update stock entity
        /// </summary>
        /// <param name="entity">Stock entity to be updated</param>
        void Update(T entity);

        /// <summary>
        /// Delete stock entity
        /// </summary>
        /// <param name="entity">Stock entity to be deleted</param>
        void Delete(T entity);

        /// <summary>
        /// Delete stock entities
        /// </summary>
        /// <param name="predicate">Predicate for entities to be deleted</param>
        void Delete(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Commit changes
        /// </summary>
        void Commit();
    }
}
