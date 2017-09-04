using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var attachments = await UnitOfWork.Get<Attachment>().GetAllAsync();
            
            return attachments.Where(a => a.ParentEntities.Any(p =>
            {
                var memberInfo = p.GetType().BaseType;
                return memberInfo != null && memberInfo.Name.ToLower().Equals(type);
            })).Select(a => a.FullPath);
        }

        public async Task<bool> HasMainPictureAsync(int id, int mainPictureId)
        {
            var existentMainPicture = await UnitOfWork.Get<Attachment>()
                .GetAsync(a => a.Type.Name.Equals("Основное изображение") &&
                               a.ParentEntities.Select(p => p.ID).Contains(id));

            return existentMainPicture != null && existentMainPicture.ID == mainPictureId;
        }

        public async Task<IEnumerable<Attachment>> GetPicturesExceptAsync(int currentMainPictureId)
        {
            var attachments = await UnitOfWork.Get<Attachment>().GetAllAsync();

            var extensions = "png,bmp,dwg,gif,ico,jpeg,jpg,pic,tif,tiff".Split(',');

            return attachments.Where(a => a.ID != currentMainPictureId &&
                                          extensions.Contains(Path.GetExtension(a.FullPath)?.TrimStart('.')));
        }
    }
}
