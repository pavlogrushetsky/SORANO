using Microsoft.AspNetCore.Mvc;

namespace SORANO.WEB.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
