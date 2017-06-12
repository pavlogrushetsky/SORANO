using SORANO.BLL.Helpers;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Linq;

namespace SORANO.BLL.Services
{
    public abstract class BaseService
    {
        protected readonly IUnitOfWork _unitOfWork;

        protected BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected void UpdateRecommendations(StockEntity from, StockEntity to, int userId)
        {
            // Remove deleted recommendations for existent entity
            to.Recommendations
                .Where(r => !from.Recommendations.Select(x => x.ID).Contains(r.ID))
                .ToList()
                .ForEach(r =>
                {
                    r.UpdateDeletedFields(userId);
                    _unitOfWork.Get<Recommendation>().Update(r);
                });

            // Add newly created recommendations to existent entity
            from.Recommendations
                .Where(r => !to.Recommendations.Select(x => x.ID).Contains(r.ID))
                .ToList()
                .ForEach(r =>
                {
                    r.ParentEntityID = to.ID;
                    r.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
                    to.Recommendations.Add(r);
                });

            // Update existent recommendations
            from.Recommendations
                .Where(r => to.Recommendations.Select(x => x.ID).Contains(r.ID))
                .ToList()
                .ForEach(r =>
                {
                    var rec = to.Recommendations.SingleOrDefault(x => x.ID == r.ID);
                    if (rec == null)
                    {
                        return;
                    }
                    rec.Comment = r.Comment;
                    rec.Value = r.Value;
                    rec.UpdateModifiedFields(userId);
                });
        }
    }
}
