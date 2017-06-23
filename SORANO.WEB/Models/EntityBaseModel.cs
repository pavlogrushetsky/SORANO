﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SORANO.WEB.Models.Recommendation;
using SORANO.WEB.Models.Attachment;

namespace SORANO.WEB.Models
{
    public abstract class EntityBaseModel
    {
        [Display(Name = "ID")]
        public virtual int ID { get; set; }

        [Display(Name = "Рекомендации")]
        public virtual List<RecommendationModel> Recommendations { get; set; } = new List<RecommendationModel>();

        [Display(Name = "Вложения")]
        public virtual List<AttachmentModel> Attachments { get; set; } = new List<AttachmentModel>();

        [Display(Name = "Создание")]
        public virtual string Created { get; set; }

        public virtual string CreatedBy { get; set; }

        [Display(Name = "Последнее изменение")]
        public virtual string Modified { get; set; }

        public virtual string ModifiedBy { get; set; }

        [Display(Name = "Удаление")]
        public virtual string Deleted { get; set; }

        public virtual string DeletedBy { get; set; }

        public virtual bool CanBeDeleted { get; set; }

        [Display(Name = "Статус")]
        public virtual bool IsDeleted { get; set; }
    }
}