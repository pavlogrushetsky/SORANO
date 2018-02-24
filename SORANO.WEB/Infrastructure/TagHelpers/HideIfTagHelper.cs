using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SORANO.WEB.Infrastructure.TagHelpers
{
    [HtmlTargetElement(Attributes = "hide-if")]
    public class HideIfTagHelper : TagHelper
    {
        public bool HideIf { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (HideIf)
                output.SuppressOutput();
        }
    }
}
