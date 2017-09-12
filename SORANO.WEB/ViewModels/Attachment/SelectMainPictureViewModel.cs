using System.Collections.Generic;

namespace SORANO.WEB.ViewModels.Attachment
{
    public class SelectMainPictureViewModel
    {
        public string ReturnUrl { get; set; }

        public int SelectedID { get; set; }

        public List<MainPictureViewModel> Pictures { get; set; }
    }
}