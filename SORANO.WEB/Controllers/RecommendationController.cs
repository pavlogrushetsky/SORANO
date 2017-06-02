using Microsoft.AspNetCore.Mvc;
using SORANO.WEB.Models.Recommendation;
using System.Collections.Generic;

namespace SORANO.WEB.Controllers
{
    public class RecommendationController : Controller
    {
        public IActionResult TableForm(List<RecommendationModel> recommendations)
        {
            return PartialView("_TableForm", recommendations);
        }
    }
}
