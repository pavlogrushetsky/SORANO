using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SORANO.WEB.Infrastructure.Filters
{
    public class ValidateModelFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            dynamic controller = context.Controller;

            context.ActionArguments.TryGetValue("model", out dynamic model);
            context.ActionArguments.TryGetValue("mainPictureFile", out var mainPicture);
            context.ActionArguments.TryGetValue("attachments", out var attachments);

            if (mainPicture is IFormFile || attachments is IFormFileCollection attachmentFiles && attachmentFiles.Count > 0)
                controller.TryValidateModel(model);

            if (!context.ModelState.IsValid)
            {               
                var result = new ViewResult
                {
                    ViewName = "Create",                    
                    TempData = controller.TempData,
                    ViewData = new ViewDataDictionary(controller.ViewData)
                    {
                        Model = model
                    }
                };

                context.Result = result;
            }
            else
            {
                await next();
            }
        }
    }
}