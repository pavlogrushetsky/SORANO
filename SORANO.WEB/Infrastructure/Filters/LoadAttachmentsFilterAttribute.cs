using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SORANO.WEB.Infrastructure.Filters
{
    public class LoadAttachmentsFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            dynamic controller = context.Controller;

            context.ActionArguments.TryGetValue("model", out dynamic model);
            context.ActionArguments.TryGetValue("mainPictureFile", out var mainPicture);
            context.ActionArguments.TryGetValue("attachments", out var attachments);

            var mainPictureFile = mainPicture as IFormFile;
            var attachmentFiles = attachments as IFormFileCollection;

            if (controller != null)
            {
                if (mainPictureFile != null)
                    await controller.LoadMainPicture(model, mainPictureFile);

                if (attachmentFiles != null && attachmentFiles.Count > 0)
                    await controller.LoadAttachments(model, attachmentFiles);
            }
                       
            await next();
        }
    }
}