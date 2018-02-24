using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SORANO.WEB.Infrastructure.TagHelpers
{
    [HtmlTargetElement("button", Attributes = "tooltip")]
    [HtmlTargetElement("a", Attributes = "tooltip")]
    public class TooltipTagHelper : TagHelper
    {
        public string Tooltip { get; set; }

        public string TooltipPlacement { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("data-toggle", "tooltip");
            output.Attributes.SetAttribute("data-placement", TooltipPlacement);
            output.Attributes.SetAttribute("title", string.Empty);
            output.Attributes.SetAttribute("data-original-title", Tooltip);           
        }
    }
}
