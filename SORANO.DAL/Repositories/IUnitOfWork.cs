using SORANO.CORE;
using System.Threading.Tasks;

namespace SORANO.DAL.Repositories
{
    public interface IUnitOfWork
    {
        StockRepository<T> Get<T>() where T : Entity, new();

        void Save();

        Task SaveAsync();
    }
}
