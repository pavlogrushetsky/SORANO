using System.Collections.Generic;
using SORANO.WEB.ViewModels.Attachment;
using SORANO.WEB.ViewModels.Recommendation;

namespace SORANO.WEB.ViewModels.Common
{
    public class BaseCreateUpdateViewModel
    {
        public int ID { get; set; }

        public bool IsUpdate { get; set; }

        public IList<RecommendationViewModel> Recommendations { get; set; } = new List<RecommendationViewModel>();

        public IList<AttachmentViewModel> Attachments { get; set; } = new List<AttachmentViewModel>();

        public MainPictureViewModel MainPicture { get; set; }
    }
}