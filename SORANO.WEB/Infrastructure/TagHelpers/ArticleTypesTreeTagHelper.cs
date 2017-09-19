using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SORANO.WEB.ViewModels.ArticleType;

namespace SORANO.WEB.Infrastructure.TagHelpers
{  
    // ReSharper disable once ClassNeverInstantiated.Global

    public class ArticleTypesTreeTagHelper : TagHelper
    {
        // ReSharper disable once CollectionNeverUpdated.Global

        public IEnumerable<ArticleTypeIndexViewModel> Elements { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "ul";

            var tree = "";

            foreach (var element in Elements)
            {
                RenderType(element, ref tree);
            }

            output.Content.SetHtmlContent(tree);
        }

        private void RenderType(ArticleTypeIndexViewModel type, ref string html)
        {        
            html += "<li id='" + type.ID + "' class='" + (type.IsDeleted ? " deleted" : "") + "'" + "><span><i class='fa fa-tag'></i>" + type.Name;

            if (type.HasChildTypes)
            {
                html += "<span class='badge' style='display:none;'>" + type.ChildTypesCount + "</span>";
            }

            html += "</span>";

            html += "<a class='btn btn-xs btn-link' data-toggle='tooltip' data-placement='right' data-original-title='Просмотреть краткую информацию' title =''><i class='fa fa-info fa-lg'></i></a>";

            if (type.HasChildTypes || type.HasArticles)
            {
                html += "<ul>";

                foreach (var childType in type.ChildTypes)
                {
                    RenderType(childType, ref html);
                }

                foreach (var article in type.Articles)
                {
                    html += "<li class='" + (article.IsDeleted ? " deleted" : "") + "'><span><i class='fa fa-barcode'></i>" + article.Name + "</span></li>";
                }              

                html += "</ul>";
            }         

            html += "</li>";           
        }
    }
}
