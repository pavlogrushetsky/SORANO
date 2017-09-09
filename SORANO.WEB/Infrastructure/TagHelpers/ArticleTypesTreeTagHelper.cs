using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SORANO.WEB.ViewModels;
using SORANO.WEB.ViewModels.ArticleType;

namespace SORANO.WEB.Infrastructure.TagHelpers
{  
    // ReSharper disable once ClassNeverInstantiated.Global

    public class ArticleTypesTreeTagHelper : TagHelper
    {
        // ReSharper disable once CollectionNeverUpdated.Global

        public List<ArticleTypeIndexViewModel> Elements { get; set; }

        public ArticleTypeModel CurrentElement { get; set; }

        public bool Detailed { get; set; }

        public bool ShowArticles { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "ul";

            var tree = "";

            Elements.ForEach(e =>
            {
                RenderType(e, ref tree);
            });

            output.Content.SetHtmlContent(tree);
        }

        private void RenderType(ArticleTypeIndexViewModel type, ref string html)
        {
            if (CurrentElement != null && (type.ID == CurrentElement.ID || CurrentElement.ChildTypes.Select(e => e.ID).Contains(type.ID)))
            {
                return;
            }            

            html += "<li id='" + type.ID + "' class='" + (type.IsSelected ? "selected" : "") + (type.IsDeleted ? " deleted" : "") + "'" + "><span><i class='fa fa-tag'></i>" + type.Name;

            if (type.ChildTypes.Any())
            {
                html += "<span class='badge' style='display:none;'>" + type.ChildTypes.Count + "</span>";
            }

            html += "</span>";

            if (Detailed)
            {
                html += "<a class='btn btn-xs btn-link' data-toggle='tooltip' data-placement='right' data-original-title='Просмотреть краткую информацию' title =''><i class='fa fa-info fa-lg'></i></a>";
            }

            if (type.ChildTypes.Any() || ShowArticles && type.Articles.Any())
            {
                html += "<ul>";

                foreach (var childType in type.ChildTypes)
                {
                    RenderType(childType, ref html);
                }

                if (ShowArticles && type.Articles.Any())
                {
                    foreach (var article in type.Articles)
                    {
                        html += "<li class='" + (article.IsDeleted ? " deleted" : "") + "'><span><i class='fa fa-barcode'></i>" + article.Name + "</span></li>";
                    }
                }                

                html += "</ul>";
            }         

            html += "</li>";           
        }
    }
}
