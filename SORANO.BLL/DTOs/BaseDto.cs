using System;
using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public abstract class BaseDto
    {
        public int ID { get; set; }

        public bool IsDeleted { get; internal set; }

        public bool CanBeDeleted { get; internal set; }

        public DateTime Created { get; internal set; }

        public DateTime Modified { get; internal set; }

        public DateTime? Deleted { get; internal set; }

        public string CreatedBy { get; internal set; }

        public string ModifiedBy { get; internal set; }

        public string DeletedBy { get; internal set; }

        public IEnumerable<RecommendationDto> Recommendations { get; set; }

        public IEnumerable<AttachmentDto> Attachments { get; set; }

        public AttachmentDto MainPicture { get; set; }
    }
}