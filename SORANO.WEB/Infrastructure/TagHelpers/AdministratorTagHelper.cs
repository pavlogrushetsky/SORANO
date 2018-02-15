using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SORANO.WEB.Infrastructure.TagHelpers
{
    [HtmlTargetElement("administrator")]
    public class AdministratorTagHelper : TagHelper
    {
        protected ClaimsPrincipal User => ViewContext.HttpContext.User;

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            var isAdministrator = User.IsInRole("administrator")
                                  || User.IsInRole("developer");
            if (!isAdministrator)
                output.SuppressOutput();
        }
    }
}
