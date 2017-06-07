using System.Threading.Tasks;

namespace SORANO.DAL.Context
{
    public interface IUnitOfWork
    {
        void Commit();

        Task CommitAsync();
    }
}
