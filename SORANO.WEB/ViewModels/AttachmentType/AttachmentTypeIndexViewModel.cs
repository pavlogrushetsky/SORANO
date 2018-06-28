using System.Collections.Generic;

namespace SORANO.WEB.ViewModels.AttachmentType
{
    public class AttachmentTypeIndexViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }

        public IEnumerable<string> Extensions { get; set; }

        public int AttachmentsCount { get; set; }

        public bool IsDeleted { get; set; }

        public bool CanBeDeleted { get; set; }

        public bool CanBeUpdated { get; set; }

        public string MimeTypes { get; set; }

        public string Modified { get; set; }

        public string ModifiedStandard { get; set; }
    }
}