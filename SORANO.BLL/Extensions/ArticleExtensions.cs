﻿using System.Linq;
using SORANO.BLL.DTOs;
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
                TypeID = model.TypeID,
                Type = model.Type.ToDto()
            };

            dto.MapDetails(model);
            dto.CanBeDeleted = !model.DeliveryItems.Any() && !model.IsDeleted;

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
                TypeID = dto.TypeID,
                Recommendations = dto.Recommendations.Select(r => r.ToEntity()).ToList(),
                Attachments = dto.Attachments.Select(a => a.ToEntity()).ToList()
            };

            entity.Attachments.Add(dto.MainPicture.ToEntity());

            return entity;
        }

        public static void UpdateFields(this Article existentArticle, Article newArticle)
        {
            existentArticle.Name = newArticle.Name;
            existentArticle.Description = newArticle.Description;
            existentArticle.Producer = newArticle.Producer;
            existentArticle.Code = newArticle.Code;
            existentArticle.Barcode = newArticle.Barcode;
            existentArticle.TypeID = newArticle.TypeID;
        }
    }
}