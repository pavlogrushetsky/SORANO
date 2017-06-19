using System.Linq;
using SORANO.CORE.StockEntities;
using SORANO.WEB.Models.ArticleType;
using System.Collections.Generic;

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
                CanBeDeleted = !type.Articles.Any(a => !a.IsDeleted) && !type.IsDeleted,      
                IsDeleted = type.IsDeleted,
                Created = type.CreatedDate.ToString("dd.MM.yyyy"),
                Modified = type.ModifiedDate.ToString("dd.MM.yyyy"),
                CreatedBy = type.CreatedByUser?.Login,
                ModifiedBy = type.ModifiedByUser?.Login,
            };

            model.Recommendations = type.Recommendations?.Select(r => r.ToModel()).ToList();
            model.Articles = type.Articles?.Select(a => a.ToModel(model)).ToList();

            if (!deep)
            {
                return model;
            }

            model.ChildTypes = type.ChildTypes?.Select(t => t.ToModel(false)).ToList();
            model.ParentType = type.ParentType?.ToModel(false);            

            return model;
        }     
        
        public static List<ArticleTypeModel> ToTree(this List<ArticleType> types)
        {
            var parents = types.Where(t => t.ParentTypeId == null).Select(t => t.ToModel(false)).ToList();

            parents.ForEach(p =>
            {
                p.FillChildren(types);
            });

            return parents;
        }

        private static void FillChildren(this ArticleTypeModel parent, List<ArticleType> children)
        {
            parent.ChildTypes = children.Where(c => c.ParentTypeId == parent.ID).Select(c => c.ToModel(false)).ToList();

            parent.ChildTypes.ToList().ForEach(c =>
            {
                c.FillChildren(children);
            });
        }

        public static ArticleType ToEntity(this ArticleTypeModel model)
        {
            return new ArticleType
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                ParentTypeId = model.ParentType != null && model.ParentType.ID > 0 ? (int?)model.ParentType.ID : null,
                Recommendations = model.Recommendations.Select(r => r.ToEntity()).ToList()
            };
        }
    }
}