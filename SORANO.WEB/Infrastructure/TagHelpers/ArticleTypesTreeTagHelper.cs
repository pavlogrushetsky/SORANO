using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SORANO.WEB.Models.Article;
using SORANO.WEB.Models.ArticleType;

namespace SORANO.WEB.Infrastructure.TagHelpers
{  
    public class ArticleTypesTreeTagHelper : TagHelper
    {
        public List<ArticleTypeModel> Elements { get; set; }

        public bool Detailed { get; set; }

        public bool ShowArticles { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            foreach (var model in Elements)
            {
                if (model.ParentType != null)
                {
                    Elements.Single(e => e.ID == model.ParentType.ID).ChildTypes.Add(model);
                }
            }

            Elements.ForEach(e =>
            {
                e.ParentType?.ChildTypes.Add(e);
            });

            output.TagName = "ul";

            var tree = "";

            var parents = Elements
                .Where(e => e.ParentType == null)
                .ToList();

            parents.ForEach(e =>
            {
                RenderType(e, ref tree);
            });

            output.Content.SetHtmlContent(tree);
        }

        private void RenderType(ArticleTypeModel type, ref string html)
        {
            html += "<li><span><i class='fa fa-tag'></i>" + type.Name;

            if (type.ChildTypes.Any())
            {
                html += "<span class='badge' style='display:none;'>" + type.ChildTypes.Count + "</span>";
            }

            html += "</span>";

            if (Detailed)
            {
                html += "<a class='btn btn-xs btn-link' title='Просмотреть подробную информацию'><i class='fa fa-info fa-lg'></i></a>";
            }

            if (type.ChildTypes.Any())
            {
                html += "<ul>";

                foreach (var childType in type.ChildTypes)
                {
                    RenderType(childType, ref html);
                }

                html += "</ul>";
            }

            if (ShowArticles && type.Articles.Any())
            {
                html += "<ul>";

                foreach (var article in type.Articles)
                {
                    html += "<li><span><i class='fa fa-barcode'></i>" + article.Name + "</span>";

                    if (Detailed)
                    {
                        html += "<a class='btn btn-xs btn-link' title='Просмотреть подробную информацию'><i class='fa fa-info fa-lg'></i></a>";
                    }

                    html += "</li>";
                }

                html += "</ul>";
            }

            html += "</li>";           
        }
    }
}
