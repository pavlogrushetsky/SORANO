using System.Linq;
using SORANO.CORE.StockEntities;
using System.Collections.Generic;
using SORANO.WEB.Models;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class ArticleTypeExtensions
    {
        public static ArticleTypeModel ToModel(this ArticleType type, bool deep = true)
        {
            var model = new ArticleTypeModel
            {
                ID = type.ID,
                Name = type.Name,
                Description = type.Description,
                ParentTypeID = type.ParentType?.ID.ToString(),
                CanBeDeleted = type.Articles.All(a => a.IsDeleted) && !type.IsDeleted,      
                IsDeleted = type.IsDeleted,
                Recommendations = type.Recommendations?.Where(r => !r.IsDeleted).Select(r => r.ToModel()).ToList(),
                MainPicture = type.Attachments?.SingleOrDefault(a => !a.IsDeleted && a.Type.Name.Equals("Основное изображение"))?.ToModel() ?? new AttachmentModel(),
                Attachments = type.Attachments?.Where(a => !a.IsDeleted && !a.Type.Name.Equals("Основное изображение")).Select(a => a.ToModel()).ToList(),
                Created = type.CreatedDate.ToString("dd.MM.yyyy"),
                Modified = type.ModifiedDate.ToString("dd.MM.yyyy"),
                Deleted = type.DeletedDate?.ToString("dd.MM.yyyy"),
                CreatedBy = type.CreatedByUser?.Login,
                ModifiedBy = type.ModifiedByUser?.Login,
                DeletedBy = type.DeletedByUser?.Login
            };

            if (!deep)
            {
                return model;
            }

            model.Articles = type.Articles?.Select(a => a.ToModel()).ToList();
            model.ChildTypes = type.ChildTypes?.Select(t => t.ToModel(false)).ToList();
            model.ParentType = type.ParentType?.ToModel(false);            

            return model;
        }     
        
        public static List<ArticleTypeModel> ToTree(this List<ArticleTypeModel> types)
        {
            var parents = types.Where(t => t.ParentType == null).ToList();

            parents.ForEach(p =>
            {
                p.FillChildren(types);
            });

            return parents;
        }

        private static void FillChildren(this ArticleTypeModel parent, List<ArticleTypeModel> children)
        {
            parent.ChildTypes = children.Where(c => c.ParentType?.ID == parent.ID).ToList();

            parent.ChildTypes.ToList().ForEach(c =>
            {
                c.FillChildren(children);
            });
        }

        public static ArticleType ToEntity(this ArticleTypeModel model)
        {
            var articleType = new ArticleType
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                ParentTypeId = string.IsNullOrEmpty(model.ParentTypeID) ? null : (int?)int.Parse(model.ParentTypeID),
                Recommendations = model.Recommendations.Select(r => r.ToEntity()).ToList(),
                Attachments = model.Attachments.Select(a => a.ToEntity()).ToList()
            };

            if (!string.IsNullOrEmpty(model.MainPicture?.Name))
            {
                articleType.Attachments.Add(model.MainPicture.ToEntity());
            }

            return articleType;
        }
    }
}