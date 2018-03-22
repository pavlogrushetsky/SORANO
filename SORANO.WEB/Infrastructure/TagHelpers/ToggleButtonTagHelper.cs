using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SORANO.WEB.Infrastructure.TagHelpers
{
    [HtmlTargetElement("button", Attributes = "State")]
    [HtmlTargetElement("a", Attributes = "State")]
    public class ToggleButtonTagHelper : TagHelper
    {
        public bool State { get; set; }

        public string TooltipTrue { get; set; }

        public string TooltipFalse { get; set; }

        public string ClassTrue { get; set; }

        public string ClassFalse { get; set; }

        public string IconTrue { get; set; }

        public string IconFalse { get; set; }

        public string IconColorTrue { get; set; }

        public string IconColorFalse { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("data-toggle", "tooltip");
            output.Attributes.SetAttribute("data-placement", "bottom");
            output.Attributes.SetAttribute("title", string.Empty);
            output.Attributes.SetAttribute("data-original-title", State ? TooltipTrue : TooltipFalse);

            if (!string.IsNullOrWhiteSpace(ClassTrue) && !string.IsNullOrWhiteSpace(ClassFalse))
            {
                var classes = output.Attributes.ContainsName("class")
                    ? output.Attributes["class"].Value
                    : string.Empty;

                var condClass = State ? ClassTrue : ClassFalse;
                output.Attributes.SetAttribute("class", $"{classes} {condClass}");
            }           

            var iconHtml = string.Empty;
            var iconStyle = string.Empty;
            if (State && !string.IsNullOrWhiteSpace(IconTrue))
            {
                if (!string.IsNullOrWhiteSpace(IconColorTrue))
                    iconStyle = $"color: {IconColorTrue};";

                iconHtml = $"<i class='fa fa-fw {IconTrue}' style='{iconStyle}'></i>";               
            }
            else if (!State && !string.IsNullOrWhiteSpace(IconFalse))
            {
                if (!string.IsNullOrWhiteSpace(IconColorFalse))
                    iconStyle = $"color: {IconColorFalse};";

                iconHtml = $"<i class='fa fa-fw {IconFalse}' style='{iconStyle}'></i>";
            }

            if (!string.IsNullOrWhiteSpace(iconHtml))
                output.Content.AppendHtml(iconHtml);
        }
    }
}
