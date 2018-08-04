using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SORANO.CORE;
// ReSharper disable UnusedMember.Global

namespace SORANO.DAL.Repositories
{
    public interface IStockRepository<T> where T : Entity, new()
    {
        #region Get All

        IQueryable<T> GetAll();

        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        #endregion

        #region Get

        T Get(int id);

        T Get(int id, params Expression<Func<T, object>>[] includeProperties);

        Task<T> GetAsync(int id);

        Task<T> GetAsync(int id, params Expression<Func<T, object>>[] includeProperties);

        T Get(Expression<Func<T, bool>> predicate);

        T Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        Task<T> GetAsync(Expression<Func<T, bool>> predicate);

        Task<T> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        #endregion

        int Count();

        Task<int> CountAsync();

        T Add(T entity);

        T Update(T entity);

        void Delete(T entity);

        void Delete(Expression<Func<T, bool>> predicate);
    }
}