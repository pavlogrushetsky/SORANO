using System;

namespace SORANO.BLL.DTOs
{
    public class DetailsDto
    {
        public bool IsDeleted { get; internal set; }

        public bool CanBeDeleted { get; internal set; }

        public DateTime Created { get; internal set; }

        public DateTime Modified { get; internal set; }

        public DateTime? Deleted { get; internal set; }

        public string CreatedBy { get; internal set; }

        public string ModifiedBy { get; internal set; }

        public string DeletedBy { get; internal set; }
    }
}