using Microsoft.AspNetCore.Mvc;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Models;
using SORANO.WEB.Models.Recommendation;

namespace SORANO.WEB.Controllers
{
    public class EntityBaseController<T> : BaseController where T : EntityBaseModel
    {
        /// <summary>
        /// Controller to perform entities controllers' base functionality
        /// </summary>
        /// <param name="userService">Users' service</param>
        public EntityBaseController(IUserService userService) : base(userService)
        {

        }

        /// <summary>
        /// Post entity model to add recommendation
        /// </summary>
        /// <param name="entity">Entity model</param>
        /// <param name="isEdit">Is editing</param>
        /// <returns>Create view</returns>
        [HttpPost]
        public virtual IActionResult AddRecommendation(T entity, bool isEdit)
        {
            ModelState.Clear();

            entity.Recommendations.Add(new RecommendationModel());

            ViewData["IsEdit"] = isEdit;

            return View("Create", entity);
        }

        /// <summary>
        /// Post entity model to remove recommendation
        /// </summary>
        /// <param name="entity">Entity model</param>
        /// <param name="isEdit">Is editing</param>
        /// <param name="num">Relative position of the recommendation</param>
        /// <returns>Create view</returns>
        [HttpPost]
        public virtual IActionResult DeleteRecommendation(T entity, bool isEdit, int num)
        {
            ModelState.Clear();

            entity.Recommendations.RemoveAt(num);

            ViewData["IsEdit"] = isEdit;

            return View("Create", entity);
        }
    }
}
