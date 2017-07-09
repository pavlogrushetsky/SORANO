using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using System.Linq;
using SORANO.WEB.Infrastructure.Extensions;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class AttachmentController : BaseController
    {
        private readonly IAttachmentService _attachmentService;

        public AttachmentController(IAttachmentService attachmentService, IUserService userService) : base(userService)
        {
            _attachmentService = attachmentService;
        }

        [HttpGet]
        public async Task<IActionResult> SelectMainPicture(int id, string returnUrl)
        {
            var attachments = await _attachmentService.GetPicturesForAsync(id);

            ViewBag.ReturnUrl = returnUrl;

            return View(attachments.Select(a => a.ToModel()).ToList());
        }
    }
}