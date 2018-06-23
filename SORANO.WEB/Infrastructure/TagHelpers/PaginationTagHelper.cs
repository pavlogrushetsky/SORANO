using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SORANO.WEB.Infrastructure.TagHelpers
{
    public class PaginationTagHelper : TagHelper
    {
        public int Pages { get; set; }

        public int Page { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Pages == 1)
                return;

            output.TagName = "ul";
            output.Attributes.Add("class", "pagination pull-right");

            for (var i = 1; i <= Pages; i++)
            {
                string html;
                if (i == Page)
                {
                    html = $@"<li class=""page active disabled""><a href=""#"">{i}</a></li>";
                    output.Content.SetHtmlContent(output.Content.GetContent() + html);
                    continue;
                }

                if (i == 1 || i == Pages || i == Page - 1 || i == Page + 1)
                {
                    html = $@"<li class=""page""><a href=""#"">{i}</a></li>";
                    output.Content.SetHtmlContent(output.Content.GetContent() + html);
                    continue;
                }

                if (i != Page - 2 && i != Page + 2)
                    continue;

                var pageClass = i == Page - 2 ? "left" : "right";

                html = $@"<li class=""page {pageClass}""><a href=""#"">...</a></li>";
                output.Content.SetHtmlContent(output.Content.GetContent() + html);
            }
        }
    }
}