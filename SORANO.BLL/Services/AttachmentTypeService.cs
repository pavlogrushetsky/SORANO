using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORANO.BLL.Dtos;
using SORANO.BLL.Extensions;
using System;

namespace SORANO.BLL.Services
{
    public class AttachmentTypeService : BaseService, IAttachmentTypeService
    {
        public AttachmentTypeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #region CRUD methods

        public async Task<ServiceResponse<IEnumerable<AttachmentTypeDto>>> GetAllAsync(bool includeMainPicture)
        {
            var response = new SuccessResponse<IEnumerable<AttachmentTypeDto>>();

            var attachmentTypes = await UnitOfWork.Get<AttachmentType>().GetAllAsync();

            response.Result = includeMainPicture
                ? attachmentTypes.Select(t => t.ToDto())
                : attachmentTypes.Where(t => !t.Name.Equals("Основное изображение")).Select(t => t.ToDto());

            return response;
        }

        public async Task<ServiceResponse<AttachmentTypeDto>> GetAsync(int id)
        {
            var attachmentType = await UnitOfWork.Get<AttachmentType>().GetAsync(a => a.ID == id);

            return attachmentType == null 
                ? new ServiceResponse<AttachmentTypeDto>(ServiceResponseStatus.NotFound) 
                : new SuccessResponse<AttachmentTypeDto>(attachmentType.ToDto());
        }

        public async Task<ServiceResponse<AttachmentTypeDto>> CreateAsync(AttachmentTypeDto attachmentType, int userId)
        {
            if (attachmentType == null)
                throw new ArgumentNullException(nameof(attachmentType));        

            var entity = attachmentType.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var saved = UnitOfWork.Get<AttachmentType>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<AttachmentTypeDto>(saved.ToDto());
        }

        public async Task<ServiceResponse<AttachmentTypeDto>> UpdateAsync(AttachmentTypeDto attachmentType, int userId)
        {
            if (attachmentType == null)
                throw new ArgumentNullException(nameof(attachmentType));          

            var existentEntity = await UnitOfWork.Get<AttachmentType>().GetAsync(t => t.ID == attachmentType.ID);

            if (existentEntity == null)
                return new ServiceResponse<AttachmentTypeDto>(ServiceResponseStatus.NotFound);

            var entity = attachmentType.ToEntity();

            existentEntity.UpdateFields(entity);
            existentEntity.UpdateModifiedFields(userId);

            var updated = UnitOfWork.Get<AttachmentType>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<AttachmentTypeDto>(updated.ToDto());
        }

        public async Task<ServiceResponse<int>> DeleteAsync(int id, int userId)
        {
            var existentAttachmentType = await UnitOfWork.Get<AttachmentType>().GetAsync(t => t.ID == id);

            if (existentAttachmentType == null)
                return new ServiceResponse<int>(ServiceResponseStatus.NotFound);

            if (existentAttachmentType.Attachments.Any())
                return new ServiceResponse<int>(ServiceResponseStatus.InvalidOperation);

            existentAttachmentType.UpdateDeletedFields(userId);

            UnitOfWork.Get<AttachmentType>().Update(existentAttachmentType);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(id);
        }

        #endregion

        public async Task<ServiceResponse<int>> GetMainPictureTypeIdAsync(int userId)
        {
            var type = await UnitOfWork.Get<AttachmentType>().GetAsync(t => t.Name.Equals("Основное изображение"));

            return new SuccessResponse<int>(type.ID);
        }

        public async Task<ServiceResponse<bool>> Exists(string name, int? attachmentTypeId, int userId)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new SuccessResponse<bool>(false);
            }

            var attachmentTypes = await UnitOfWork.Get<AttachmentType>().FindByAsync(t => t.Name.Equals(name) && t.ID != attachmentTypeId);

            return new SuccessResponse<bool>(attachmentTypes.Any());
        }
    }
}
