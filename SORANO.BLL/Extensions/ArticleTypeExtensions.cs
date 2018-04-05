﻿using System.Collections.Generic;
using System.Linq;
using SORANO.BLL.Dtos;
using SORANO.CORE.StockEntities;

namespace SORANO.BLL.Extensions
{
    internal static class ArticleTypeExtensions
    {
        public static ArticleTypeDto ToDto(this ArticleType model, string term = null)
        {
            var dto = new ArticleTypeDto
            {
                ID = model.ID,
                Name = model.Name,
                Description = model.Description,
                TypeID = model.ParentType?.ID,
                Type = model.ParentType?.ToDto(),
                ChildTypes = model.ChildTypes.Filter(term).Select(t => t.ToDto()),
                Articles = model.Articles.Select(a => a.ToDto())
            };

            dto.MapDetails(model);
            dto.CanBeDeleted = model.Articles.All(a => a.IsDeleted) && !model.IsDeleted;

            return dto;
        }

        public static ArticleType ToEntity(this ArticleTypeDto dto)
        {
            var entity = new ArticleType
            {
                ID = dto.ID,
                Name = dto.Name,
                Description = dto.Description,
                ParentTypeId = dto.TypeID,
                Recommendations = dto.Recommendations.Select(r => r.ToEntity()).ToList(),
                Attachments = dto.Attachments.Select(a => a.ToEntity()).ToList()
            };

            if (!string.IsNullOrEmpty(dto.MainPicture?.FullPath))
                entity.Attachments.Add(dto.MainPicture.ToEntity());

            return entity;
        }

        public static void UpdateFields(this ArticleType existentArticleType, ArticleType newArticleType)
        {
            existentArticleType.Name = newArticleType.Name;
            existentArticleType.Description = newArticleType.Description;
            existentArticleType.ParentTypeId = newArticleType.ParentTypeId;
        }

        public static IEnumerable<ArticleType> Filter(this IEnumerable<ArticleType> types, string term)
        {
            return string.IsNullOrWhiteSpace(term) 
                ? types 
                : types.Where(t => t.Filter(term) || t.ChildTypes.Filter(term).Any());
        }

        private static bool Filter(this ArticleType type, string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return true;

            return type.Name.ToLower().Contains(term)
                   || !string.IsNullOrWhiteSpace(type.Description) && type.Description.ToLower().Contains(term);
        }
    }
}