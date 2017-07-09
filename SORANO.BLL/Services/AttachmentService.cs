using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;

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

        public async Task<IEnumerable<Attachment>> GetPicturesForAsync(int id)
        {
            var attachments = await _unitOfWork.Get<Attachment>().GetAllAsync();

            var extensions = "bmp,dwg,gif,ico,jpeg,jpg,pic,tif,tiff".Split(',');

            return attachments.Where(a => a.ParentEntities.Any(p => p.ID == id) &&
                                          extensions.Contains(Path.GetExtension(a.FullPath)));
        }
    }
}
