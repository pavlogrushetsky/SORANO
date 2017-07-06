using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Threading.Tasks;

namespace SORANO.BLL.Services
{
    public class AttachmentService : BaseService, IAttachmentService
    {
        public AttachmentService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<string>> GetAllForAsync(string type)
        {
            var attachments = await _unitOfWork.Get<Attachment>().GetAllAsync();
            
            return attachments.Where(a => a.ParentEntities.Any(p =>
            {
                var memberInfo = p.GetType().BaseType;
                return memberInfo != null && memberInfo.Name.ToLower().Equals(type);
            })).Select(a => a.FullPath);
        }
    }
}
