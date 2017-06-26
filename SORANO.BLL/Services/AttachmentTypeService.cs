﻿using SORANO.BLL.Helpers;
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
            var attachmentTypes = await _unitOfWork.Get<AttachmentType>().GetAllAsync();

            return attachmentTypes;
        }

        public async Task<AttachmentType> GetAsync(int id)
        {
            return await _unitOfWork.Get<AttachmentType>().GetAsync(a => a.ID == id);
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

            var attachmentTypes = await _unitOfWork.Get<AttachmentType>().FindByAsync(t => t.Name.Equals(attachmentType.Name));

            if (attachmentTypes.Any())
            {
                throw new Exception(Resource.AttachmentTypeWithSameNameException);
            }

            var user = await _unitOfWork.Get<User>().GetAsync(s => s.ID == userId);

            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundException);
            }

            attachmentType.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var saved = _unitOfWork.Get<AttachmentType>().Add(attachmentType);

            await _unitOfWork.SaveAsync();

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

            var attachmentTypes = await _unitOfWork.Get<AttachmentType>().FindByAsync(t => t.Name.Equals(attachmentType.Name) && t.ID != attachmentType.ID);

            if (attachmentTypes.Any())
            {
                throw new Exception(Resource.AttachmentTypeWithSameNameException);
            }

            var user = await _unitOfWork.Get<User>().GetAsync(u => u.ID == userId);

            if (user == null)
            {
                throw new ObjectNotFoundException(Resource.UserNotFoundException);
            }

            var existentAttachmentType = await _unitOfWork.Get<AttachmentType>().GetAsync(t => t.ID == attachmentType.ID);

            if (existentAttachmentType == null)
            {
                throw new ObjectNotFoundException(Resource.AttachmentTypeNotFoundException);
            }

            existentAttachmentType.Name = attachmentType.Name;
            existentAttachmentType.Comment = attachmentType.Comment;

            existentAttachmentType.UpdateModifiedFields(userId);

            var updated = _unitOfWork.Get<AttachmentType>().Update(existentAttachmentType);

            await _unitOfWork.SaveAsync();

            return updated;
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var existentAttachmentType = await _unitOfWork.Get<AttachmentType>().GetAsync(t => t.ID == id);

            if (existentAttachmentType.Attachments.Any())
            {
                throw new Exception(Resource.AttachmentTypeCannotBeDeletedException);
            }

            existentAttachmentType.UpdateDeletedFields(userId);

            _unitOfWork.Get<AttachmentType>().Update(existentAttachmentType);

            await _unitOfWork.SaveAsync();
        }
    }
}