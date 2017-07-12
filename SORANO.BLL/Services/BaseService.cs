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
        }

        protected void UpdateAttachments(StockEntity from, StockEntity to, int userId)
        {
            // Remove deleted attachments for existent entity
            to.Attachments
                .Where(a => !from.Attachments.Select(x => x.ID).Contains(a.ID))
                .ToList()
                .ForEach(a =>
                {
                    a.ParentEntities.Remove(to);
                    _unitOfWork.Get<Attachment>().Delete(a);
                });

            // Update existent attachments
            from.Attachments
                .Where(a => to.Attachments.Select(x => x.ID).Contains(a.ID))
                .ToList()
                .ForEach(a =>
                {
                    var att = to.Attachments.SingleOrDefault(x => x.ID == a.ID);
                    if (att == null)
                    {
                        return;
                    }
                    att.Name = a.Name;
                    att.Description = a.Description;
                    att.FullPath = a.FullPath;
                    att.AttachmentTypeID = a.AttachmentTypeID;
                    att.UpdateModifiedFields(userId);
                });

            // Add newly created attachments to existent entity
            from.Attachments
                .Where(a => !to.Attachments.Select(x => x.ID).Contains(a.ID))
                .ToList()
                .ForEach(a =>
                {
                    a.ParentEntities.Add(to);
                    a.UpdateCreatedFields(userId).UpdateModifiedFields(userId);
                    to.Attachments.Add(a);
                });            
        }
    }
}
