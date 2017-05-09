using SORANO.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SORANO.DAL.Repositories.Abstract
{
    /// <summary>
    /// Generic abstract repository for the stock entities
    /// </summary>
    /// <typeparam name="T">Stock entity type</typeparam>
    public interface IEntityRepository<T> where T : Entity, new()
    {
        /// <summary>
        /// Get all stock entities
        /// </summary>
        /// <returns>Stock entities</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Get all stock entities asyncronously
        /// </summary>
        /// <returns>Stock entities</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Get all stock entities including specified properties
        /// </summary>
        /// <param name="properties">Properties to include in selection</param>
        /// <returns>Stock entities</returns>
        IEnumerable<T> GetAll(params Expression<Func<T, object>>[] properties);

        /// <summary>
        /// Get all stock entities asyncronously including specified properties
        /// </summary>
        /// <param name="properties">Properties to include in selection</param>
        /// <returns>Stock entities</returns>
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] properties);

        /// <summary>
        /// Get single stock entity
        /// </summary>
        /// <param name="id">Unique identifier of an entity</param>
        /// <returns>Stock entity</returns>
        T Get(int id);

        /// <summary>
        /// Get single stock entity asyncronously
        /// </summary>
        /// <param name="id">Unique identifier of an entity</param>
        /// <returns>Stock entity</returns>
        Task<T> GetAsync(int id);

        /// <summary>
        /// Get single stock entity for which the specified predicate is true
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>Stock entity</returns>
        T Get(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Get single stock entity asyncronously for which the specified predicate is true
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>Stock entity</returns>
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Get single stock entity for which the specified predicate is true including specified properties
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="properties">Properties to include in selection</param>
        /// <returns>Stock Entity</returns>
        T Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] properties);

        /// <summary>
        /// Get single stock entity asyncronously for which the specified predicate is true including specified properties
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="properties">Properties to include in selection</param>
        /// <returns>Stock Entity</returns>
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] properties);

        /// <summary>
        /// Find entities by specified predicate
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>Stock entities</returns>
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Find entities by specified predicate asyncronously
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>Stock entities</returns>
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Get count of stock entities of specified type
        /// </summary>
        /// <returns>Count of entities</returns>
        int Count();

        /// <summary>
        /// Get count of stock entities of specified type asyncronously
        /// </summary>
        /// <returns>Count of entities</returns>
        Task<int> CountAsync();

        /// <summary>
        /// Add stock entity
        /// </summary>
        /// <param name="entity">Stock entity to be added</param>
        /// <returns>Added entity</returns>
        T Add(T entity);

        /// <summary>
        /// Add stock entity asyncronously
        /// </summary>
        /// <param name="entity">Stock entity to be added</param>
        /// <returns>Added entity</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Update stock entity
        /// </summary>
        /// <param name="entity">Stock entity to be updated</param>
        /// <returns>Updated entity</returns>  
        T Update(T entity);

        /// <summary>
        /// Update stock entity asyncronously
        /// </summary>
        /// <param name="entity">Stock entity to be updated</param>
        /// <returns>Updated entity</returns>  
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Delete stock entity
        /// </summary>
        /// <param name="entity">Stock entity to be deleted</param>
        void Delete(T entity);

        /// <summary>
        /// Delete stock entity asyncronously
        /// </summary>
        /// <param name="entity">Stock entity to be deleted</param>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Delete stock entities
        /// </summary>
        /// <param name="predicate">Predicate for entities to be deleted</param>
        void Delete(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Delete stock entities asyncronously
        /// </summary>
        /// <param name="predicate">Predicate for entities to be deleted</param>
        Task DeleteAsync(Expression<Func<T, bool>> predicate);
    }
}
