using System.Linq;
using SORANO.CORE.StockEntities;
using SORANO.WEB.Models.Article;
using SORANO.WEB.Models.Attachment;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class ArticleExtensions
    {
        public static ArticleModel ToModel(this Article article)
        {
            return new ArticleModel
            {
                ID = article.ID,
                Code = article.Code,
                Name = article.Name,
                Producer = article.Producer,
                Description = article.Description,
                Barcode = article.Barcode,                
                Type = article.Type.ToModel(false),
                Recommendations = article.Recommendations?.Where(r => !r.IsDeleted).Select(r => r.ToModel()).ToList(),
                MainPicture = article.Attachments?.SingleOrDefault(a => !a.IsDeleted && a.Type.Name.Equals("Основное изображение"))?.ToModel() ?? new AttachmentModel(),
                Attachments = article.Attachments?.Where(a => !a.IsDeleted && !a.Type.Name.Equals("Основное изображение")).Select(a => a.ToModel()).ToList(),
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
            var article = new Article
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                Producer = model.Producer,
                Code = model.Code,
                Barcode = model.Barcode,
                TypeID = model.Type.ID,
                Recommendations = model.Recommendations.Select(r => r.ToEntity()).ToList(),
                Attachments = model.Attachments.Select(a => a.ToEntity()).ToList()
            };

            if (!string.IsNullOrEmpty(model.MainPicture?.Name))
            {
                article.Attachments.Add(model.MainPicture.ToEntity());
            }            

            return article;
        }
    }
}