using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SORANO.WEB.Infrastructure.TagHelpers
{
    [HtmlTargetElement(Attributes = "disable-if")]
    public class DisableIfTagHelper : TagHelper
    {
        public bool DisableIf { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (DisableIf)
                output.Attributes.SetAttribute("disabled", "disabled");
        }
    }
}
