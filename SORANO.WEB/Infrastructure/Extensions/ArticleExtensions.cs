using System.Linq;
using SORANO.CORE.StockEntities;
using SORANO.WEB.Models.Article;
using SORANO.WEB.Models.ArticleType;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class ArticleExtensions
    {
        public static ArticleModel ToModel(this Article article, ArticleTypeModel type)
        {
            return new ArticleModel
            {
                ID = article.ID,
                Code = article.Code,
                Name = article.Name,
                Producer = article.Producer,
                Description = article.Description,
                Barcode = article.Barcode,                
                Type = type,
                Recommendations = article.Recommendations?.Select(r => r.ToModel()).ToList(),
                CanBeDeleted = !article.DeliveryItems.Any() && !article.IsDeleted,
                IsDeleted = article.IsDeleted,
                Created = article.CreatedDate.ToString("dd.MM.yyyy"),
                Modified = article.ModifiedDate.ToString("dd.MM.yyyy"),
                Deleted = article.DeletedDate?.ToString("dd.MM.yyyy"),
                CreatedBy = article.CreatedByUser?.Login,
                ModifiedBy = article.ModifiedByUser?.Login,
                DeletedBy = article.DeletedByUser?.Login
            };
        }

        public static Article ToEntity(this ArticleModel model)
        {
            return new Article
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                Producer = model.Producer,
                Code = model.Code,
                Barcode = model.Barcode,
                TypeID = model.Type.ID,
                Recommendations = model.Recommendations.Select(r => r.ToEntity()).ToList()
            };
        }
    }
}