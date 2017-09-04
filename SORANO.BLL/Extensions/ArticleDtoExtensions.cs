using System.Linq;
using SORANO.BLL.DTOs;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class ArticleDtoExtensions
    {
        public static ArticleDto ToDto(this Article model)
        {
            return new ArticleDto
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                Producer = model.Producer,
                Code = model.Code,
                Barcode = model.Barcode,
                TypeID = model.TypeID,
                Type = model.Type.ToDto(),
                IsDeleted = model.IsDeleted,
                CanBeDeleted = !model.DeliveryItems.Any() && !model.IsDeleted,
                Created = model.CreatedDate,
                Modified = model.ModifiedDate,
                Deleted = model.DeletedDate,
                CreatedBy = model.CreatedByUser?.Login,
                ModifiedBy = model.ModifiedByUser?.Login,
                DeletedBy = model.DeletedByUser?.Login,
                Recommendations = model.Recommendations?
                    .Where(r => !r.IsDeleted)
                    .Select(r => r.ToDto()),
                Attachments = model.Attachments?
                    .Where(a => a.IsDeleted && !a.Type.Name.Equals("Основное изображение"))
                    .Select(a => a.ToDto()),
                MainPicture = model.Attachments?
                    .SingleOrDefault(a => !a.IsDeleted && a.Type.Name.Equals("Основное изображение"))?
                    .ToDto() ?? new AttachmentDto()
            };
        }
    }
}