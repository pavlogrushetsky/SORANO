﻿using SORANO.BLL.Services.Abstract;
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

        public ServiceResponse<IEnumerable<AttachmentTypeDto>> GetAll(bool includeMainPicture)
        {
            var attachmentTypes = UnitOfWork.Get<AttachmentType>()
                .GetAll(at => includeMainPicture || !at.Name.Equals("Основное изображение"))
                .OrderByDescending(at => at.ModifiedDate)
                .Select(at => new AttachmentTypeDto
                {
                    ID = at.ID,
                    Name = at.Name,
                    Comment = at.Comment,
                    Extensions = at.Extensions,
                    AttachmentsCount = at.TypeAttachments.Count,
                    Modified = at.ModifiedDate,
                    CanBeUpdated = !at.Name.Equals("Основное изображение"),
                    CanBeDeleted = !at.IsDeleted &&
                                   !at.TypeAttachments.Any() &&
                                   !at.Name.Equals("Основное изображение"),
                    IsDeleted = at.IsDeleted
                })
                .ToList();

            return new SuccessResponse<IEnumerable<AttachmentTypeDto>>(attachmentTypes);
        }

        public async Task<ServiceResponse<AttachmentTypeDto>> GetAsync(int id)
        {
            var attachmentType = await UnitOfWork.Get<AttachmentType>().GetAsync(a => a.ID == id);

            return attachmentType == null 
                ? new ServiceResponse<AttachmentTypeDto>(ServiceResponseStatus.NotFound) 
                : new SuccessResponse<AttachmentTypeDto>(attachmentType.ToDto());
        }

        public async Task<ServiceResponse<int>> CreateAsync(AttachmentTypeDto attachmentType, int userId)
        {
            if (attachmentType == null)
                throw new ArgumentNullException(nameof(attachmentType));

            var attachmentTypes = UnitOfWork.Get<AttachmentType>().GetAll(t => t.Name.Equals(attachmentType.Name) && t.ID != attachmentType.ID);

            if (attachmentTypes.Any())
                return new ServiceResponse<int>(ServiceResponseStatus.AlreadyExists);

            var entity = attachmentType.ToEntity();

            entity.UpdateCreatedFields(userId).UpdateModifiedFields(userId);

            var added = UnitOfWork.Get<AttachmentType>().Add(entity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<int>(added.ID);
        }

        public async Task<ServiceResponse<AttachmentTypeDto>> UpdateAsync(AttachmentTypeDto attachmentType, int userId)
        {
            if (attachmentType == null)
                throw new ArgumentNullException(nameof(attachmentType));          

            var existentEntity = await UnitOfWork.Get<AttachmentType>().GetAsync(t => t.ID == attachmentType.ID);

            if (existentEntity == null)
                return new ServiceResponse<AttachmentTypeDto>(ServiceResponseStatus.NotFound);

            var attachmentTypes = UnitOfWork.Get<AttachmentType>().GetAll(t => t.Name.Equals(attachmentType.Name) && t.ID != attachmentType.ID);

            if (attachmentTypes.Any())
                return new ServiceResponse<AttachmentTypeDto>(ServiceResponseStatus.AlreadyExists);

            var entity = attachmentType.ToEntity();

            existentEntity.UpdateFields(entity);
            existentEntity.UpdateModifiedFields(userId);

            UnitOfWork.Get<AttachmentType>().Update(existentEntity);

            await UnitOfWork.SaveAsync();

            return new SuccessResponse<AttachmentTypeDto>();
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
            var type = await UnitOfWork.Get<AttachmentType>()
                .GetAsync(t => t.Name.Equals("Основное изображение"));

            return new SuccessResponse<int>(type.ID);
        }

        public ServiceResponse<IEnumerable<AttachmentTypeDto>> GetAll(string searchTerm)
        {
            var term = searchTerm?.ToLower();

            var attachmentTypes = UnitOfWork.Get<AttachmentType>()
                .GetAll(at => !at.Name.Equals("Основное изображение") && 
                              term == null || 
                              at.Name.ToLower().Contains(term) || 
                              at.Comment.ToLower().Contains(term) || 
                              at.Extensions.ToLower().Contains(term))
                .OrderByDescending(at => at.Name)
                .ToList();           

            return new SuccessResponse<IEnumerable<AttachmentTypeDto>>(attachmentTypes.Select(at => at.ToDto()));
        }
    }
}
