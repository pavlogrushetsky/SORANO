using System.Collections.Generic;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Linq;
using SORANO.CORE.AccountEntities;
using System.Threading.Tasks;

namespace SORANO.BLL.Services
{
    public abstract class BaseService
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected BaseService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        protected async Task<bool> IsAccessDenied(int userId)
        {
            var user = await UnitOfWork.Get<User>().GetAsync(u => u.ID == userId);

            return user == null || user.IsBlocked;
        }      

        protected IEnumerable<Attachment> GetAttachments(int entityId)
        {
            return UnitOfWork.Get<Attachment>()
                .GetAll(a => a.ParentEntities.Any(e => e.ID == entityId),
                    a => a.Type, a => a.ParentEntities)
                .ToList();
        }

        protected IEnumerable<Recommendation> GetRecommendations(int entityId)
        {
            return UnitOfWork.Get<Recommendation>()
                .GetAll(r => r.ParentEntityID == entityId, r => r.ParentEntity)
                .ToList();
        }

        protected IEnumerable<Recommendation> GetRecommendations(IEnumerable<int> entitiesId)
        {
            return UnitOfWork.Get<Recommendation>()
                .GetAll(r => entitiesId.Contains(r.ParentEntityID), r => r.ParentEntity)
                .ToList();
        }
    }
}
