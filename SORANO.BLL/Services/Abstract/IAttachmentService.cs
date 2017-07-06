using System.Collections.Generic;
using System.Threading.Tasks;

namespace SORANO.BLL.Services.Abstract
{
    public interface IAttachmentService
    {
        Task<IEnumerable<string>> GetAllForAsync(string type);
    }
}
