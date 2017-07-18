using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Models;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager,editor,user")]
    public class DeliveryController : EntityBaseController<DeliveryModel>
    {
        public DeliveryController(IUserService userService,
            IHostingEnvironment hostingEnvironment,
            IAttachmentTypeService attachmentTypeService,
            IAttachmentService attachmentService,
            IMemoryCache memoryCache) : base(userService, hostingEnvironment, attachmentTypeService, attachmentService,
            memoryCache)
        {
            
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}