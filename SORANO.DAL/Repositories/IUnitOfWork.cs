using SORANO.CORE;
using System;
using System.Threading.Tasks;

namespace SORANO.DAL.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IStockRepository<T> Get<T>() where T : Entity, new();

        void Save();

        Task SaveAsync();
    }
}
