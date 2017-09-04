using SORANO.BLL.Helpers;
using SORANO.BLL.Properties;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.AccountEntities;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SORANO.BLL.Services
{
    public class AttachmentTypeService : BaseService, IAttachmentTypeService
    {
        public AttachmentTypeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<AttachmentType>> GetAllAsync()
        {
            var attachmentTypes = await UnitOfWork.Get<AttachmentType>().GetAllAsync();

            return attachmentTypes;
        }

        public async Task<AttachmentType> GetAsync(int id)
        {
            return await UnitOfWork.Get<AttachmentType>().GetAsync(a => a.ID == id);
        }

        public async Task<AttachmentType> CreateAsync(AttachmentType attachmentType, int userId)
        {
            if (attachmentType == null)
            {
                throw new ArgumentNullException(nameof(attachmentType), Resource.AttachmentTypeCannotBeNullException);
            }

            if (attachmentType.ID != 0)
            {
                throw new ArgumentException(Resource.AttachmentTypeInvalidIdentifierException, nameof(attachmentType.ID));
            }           

            var user = await UnitOfWork.Get<User>().GetAsync(s => s.ID == userId);

            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundMessage);
            }

            attachmentType.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var saved = UnitOfWork.Get<AttachmentType>().Add(attachmentType);

            await UnitOfWork.SaveAsync();

            return saved;
        }

        public async Task<AttachmentType> UpdateAsync(AttachmentType attachmentType, int userId)
        {
            if (attachmentType == null)
            {
                throw new ArgumentNullException(nameof(attachmentType), Resource.AttachmentTypeCannotBeNullException);
            }

            if (attachmentType.ID <= 0)
            {
                throw new ArgumentException(Resource.AttachmentTypeInvalidIdentifierException, nameof(attachmentType.ID));
            }           

            var user = await UnitOfWork.Get<User>().GetAsync(u => u.ID == userId);

            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundMessage);
            }

            var existentAttachmentType = await UnitOfWork.Get<AttachmentType>().GetAsync(t => t.ID == attachmentType.ID);

            if (existentAttachmentType == null)
            {
                throw new ObjectNotFoundException(Resource.AttachmentTypeNotFoundException);
            }

            existentAttachmentType.Name = attachmentType.Name;
            existentAttachmentType.Comment = attachmentType.Comment;
            existentAttachmentType.Extensions = attachmentType.Extensions;

            existentAttachmentType.UpdateModifiedFields(userId);

            var updated = UnitOfWork.Get<AttachmentType>().Update(existentAttachmentType);

            await UnitOfWork.SaveAsync();

            return updated;
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var existentAttachmentType = await UnitOfWork.Get<AttachmentType>().GetAsync(t => t.ID == id);

            if (existentAttachmentType.Attachments.Any())
            {
                throw new Exception(Resource.AttachmentTypeCannotBeDeletedException);
            }

            existentAttachmentType.UpdateDeletedFields(userId);

            UnitOfWork.Get<AttachmentType>().Update(existentAttachmentType);

            await UnitOfWork.SaveAsync();
        }

        public async Task<int> GetMainPictureTypeIDAsync()
        {
            var type = await UnitOfWork.Get<AttachmentType>().GetAsync(t => t.Name.Equals("Основное изображение"));

            return type.ID;
        }

        public async Task<bool> Exists(string name, int attachmentTypeId = 0)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            var attachmentTypes = await UnitOfWork.Get<AttachmentType>().FindByAsync(t => t.Name.Equals(name) && t.ID != attachmentTypeId);

            return attachmentTypes.Any();
        }
    }
}
