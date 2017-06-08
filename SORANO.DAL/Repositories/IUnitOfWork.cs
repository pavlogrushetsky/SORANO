using SORANO.CORE;
using System;
using System.Threading.Tasks;

namespace SORANO.DAL.Repositories
{
    /// <summary>
    /// Interface for unit of work
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Get repository instance
        /// </summary>
        /// <typeparam name="T">Type parameter</typeparam>
        /// <returns>Instance of generic repository</returns>
        StockRepository<T> Get<T>() where T : Entity, new();

        /// <summary>
        /// Save changes
        /// </summary>
        void Save();

        /// <summary>
        /// Save changes asynchronously
        /// </summary>
        /// <returns></returns>
        Task SaveAsync();
    }
}
