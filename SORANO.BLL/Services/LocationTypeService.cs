using SORANO.BLL.Services.Abstract;
using SORANO.DAL.Repositories;

namespace SORANO.BLL.Services
{
    public class LocationTypeService : BaseService, ILocationTypeService
    {
        public LocationTypeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
