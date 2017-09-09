﻿using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using SORANO.BLL.Dtos;
using SORANO.BLL.Extensions;

namespace SORANO.BLL.Services
{
    public class AttachmentService : BaseService, IAttachmentService
    {
        public AttachmentService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<ServiceResponse<IEnumerable<string>>> GetAllForAsync(string type, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<IEnumerable<string>>();

            var attachments = await UnitOfWork.Get<Attachment>().GetAllAsync();
            
            return new SuccessResponse<IEnumerable<string>>(attachments.Where(a => a.ParentEntities.Any(p =>
            {
                var memberInfo = p.GetType().BaseType;
                return memberInfo != null && memberInfo.Name.ToLower().Equals(type);
            })).Select(a => a.FullPath));
        }

        public async Task<ServiceResponse<bool>> HasMainPictureAsync(int id, int mainPictureId, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<bool>();

            var existentMainPicture = await UnitOfWork.Get<Attachment>()
                .GetAsync(a => a.Type.Name.Equals("Основное изображение") &&
                               a.ParentEntities.Select(p => p.ID).Contains(id));

            return new SuccessResponse<bool>(existentMainPicture != null && existentMainPicture.ID == mainPictureId);
        }

        public async Task<ServiceResponse<IEnumerable<AttachmentDto>>> GetPicturesExceptAsync(int currentMainPictureId, int userId)
        {
            if (await IsAccessDenied(userId))
                return new AccessDeniedResponse<IEnumerable<AttachmentDto>>();

            var attachments = await UnitOfWork.Get<Attachment>().GetAllAsync();

            var extensions = "png,bmp,dwg,gif,ico,jpeg,jpg,pic,tif,tiff".Split(',');

            return new SuccessResponse<IEnumerable<AttachmentDto>>(attachments
                .Where(a => a.ID != currentMainPictureId 
                    && extensions.Contains(Path.GetExtension(a.FullPath)?.TrimStart('.'))).Select(a => a.ToDto()));
        }
    }
}
