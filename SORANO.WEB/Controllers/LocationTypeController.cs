using Microsoft.AspNetCore.Authorization;
using SORANO.BLL.Services.Abstract;

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "developer,administrator,manager")]
    public class LocationTypeController : BaseController
    {
        private readonly ILocationTypeService _locationTypeService;

        public LocationTypeController(ILocationTypeService locationTypeService, IUserService userService) : base(userService)
        {
            _locationTypeService = locationTypeService;
        }

        #region GET Actions

        #endregion

        #region POST Actions

        #endregion
    }
}
