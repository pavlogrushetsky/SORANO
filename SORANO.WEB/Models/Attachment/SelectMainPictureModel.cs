using System.Collections.Generic;

namespace SORANO.WEB.Models.Attachment
{
    public class SelectMainPictureModel
    {
        public List<AttachmentModel> Pictures { get; set; } = new List<AttachmentModel>();

        public string ReturnUrl { get; set; }

        public int SelectedID { get; set; }
    }
}