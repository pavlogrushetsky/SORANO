﻿using System.Collections.Generic;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Linq;
using SORANO.BLL.Extensions;
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

        protected void UpdateRecommendations(StockEntity from, StockEntity to, int userId)
        {
            // Remove deleted recommendations for existent entity
            to.Recommendations
                .Where(r => !from.Recommendations.Select(x => x.ID).Contains(r.ID))
                .ToList()
                .ForEach(r =>
                {
                    r.UpdateDeletedFields(userId);
                    UnitOfWork.Get<Recommendation>().Update(r);
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
                    UnitOfWork.Get<Attachment>().Delete(a);
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

        protected IEnumerable<Attachment> GetAttachments(int entityId)
        {
            return UnitOfWork.Get<Attachment>()
                .GetAll(a => a.ParentEntities.Any(e => e.ID == entityId),
                    a => a.Type)
                .ToList();
        }

        protected IEnumerable<Recommendation> GetRecommendations(int entityId)
        {
            return UnitOfWork.Get<Recommendation>()
                .GetAll(r => r.ParentEntityID == entityId)
                .ToList();
        }
    }
}
