using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Models;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager,editor,user")]
    public class SaleController : BaseController
    {
        public SaleController(IUserService userService) : base(userService)
        {
                
        }

        #region GET Actions

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Sale(int articleId, int currentLocationId, int maxCount, string returnUrl)
        {
            return View(new SaleModel());
        }

        #endregion
    }
}