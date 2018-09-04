using System.Linq;
using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class ArticleExtensions
    {
        public static ArticleDto ToDto(this Article model)
        {
            var dto = new ArticleDto
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                Producer = model.Producer,
                Code = model.Code,
                Barcode = model.Barcode,
                RecommendedPrice = model.RecommendedPrice,
                TypeID = model.TypeID,
                Type = model.Type?.ToDto(),
                DeliveryItems = model.DeliveryItems?.Where(di => !di.IsDeleted && !di.Delivery.IsDeleted).Select(i => i.ToDto())
            };

            dto.MapDetails(model);
            dto.CanBeDeleted = (!model.DeliveryItems?.Any() ?? false) && !model.IsDeleted;

            return dto;
        }

        public static Article ToEntity(this ArticleDto dto)
        {
            var entity = new Article
            {
                ID = dto.ID,
                Name = dto.Name,
                Description = dto.Description,
                Producer = dto.Producer,
                Code = dto.Code,
                Barcode = dto.Barcode,
                RecommendedPrice = dto.RecommendedPrice,
                TypeID = dto.TypeID,
                Recommendations = dto.Recommendations.Select(r => r.ToEntity()).ToList(),
                Attachments = dto.Attachments.Select(a => a.ToEntity()).ToList()
            };

            if (!string.IsNullOrEmpty(dto.MainPicture?.FullPath))
                entity.Attachments.Add(dto.MainPicture.ToEntity());

            return entity;
        }

        public static Article UpdateFields(this Article existentArticle, Article newArticle)
        {
            existentArticle.Name = newArticle.Name;
            existentArticle.Description = newArticle.Description;
            existentArticle.Producer = newArticle.Producer;
            existentArticle.Code = newArticle.Code;
            existentArticle.Barcode = newArticle.Barcode;
            existentArticle.RecommendedPrice = newArticle.RecommendedPrice;
            existentArticle.TypeID = newArticle.TypeID;

            return existentArticle;
        }
    }
}