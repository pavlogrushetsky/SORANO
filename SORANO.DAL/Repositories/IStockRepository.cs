using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SORANO.CORE;

namespace SORANO.DAL.Repositories
{
    public interface IStockRepository<T> where T : Entity, new()
    {
        IEnumerable<T> GetAll();

        Task<IEnumerable<T>> GetAllAsync();

        T Get(int id);

        Task<T> GetAsync(int id);

        T Get(Expression<Func<T, bool>> predicate);

        Task<T> GetAsync(Expression<Func<T, bool>> predicate);

        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);

        int Count();

        Task<int> CountAsync();

        T Add(T entity);

        T Update(T entity);

        void Delete(T entity);

        void Delete(Expression<Func<T, bool>> predicate);
    }
}