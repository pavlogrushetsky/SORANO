using System.Linq;
using SORANO.CORE.StockEntities;
using SORANO.WEB.Models.ArticleType;

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
                CanBeDeleted = !type.Articles.Any(),              
                Created = type.CreatedDate.ToString("dd.MM.yyyy"),
                Modified = type.ModifiedDate.ToString("dd.MM.yyyy"),
                CreatedBy = type.CreatedByUser?.Login,
                ModifiedBy = type.ModifiedByUser?.Login,
            };

            if (!deep)
            {
                return model;
            }

            model.ChildTypes = type.ChildTypes?.Select(t => t.ToModel(false)).ToList();
            model.ParentType = type.ParentType?.ToModel(false);
            model.Recommendations = type.Recommendations?.Select(r => r.ToModel()).ToList();
            model.Articles = type.Articles?.Select(a => a.ToModel(model)).ToList();

            return model;
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