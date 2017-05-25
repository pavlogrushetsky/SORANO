using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SORANO.WEB.Models.Article;

namespace SORANO.WEB.Infrastructure.TagHelpers
{  
    public class ArticleTypesTreeTagHelper : TagHelper
    {
        public List<ArticleTypeModel> Elements { get; set; }

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
            html += "<li><span><i></i>" + type.Name + "</span>";

            if (type.ChildTypes.Any())
            {
                html += "<ul>";

                foreach (var childType in type.ChildTypes)
                {
                    RenderType(childType, ref html);
                }

                html += "</ul>";
            }

            html += "</li>";           
        }
    }
}
