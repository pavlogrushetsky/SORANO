using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.AttachmentType
{
    public class AttachmentTypeCreateUpdateViewModel : BaseCreateUpdateViewModel
    {
        [Display(Name = "Название *")]
        public string Name { get; set; }

        [Display(Name = "Коментарий")]
        public string Comment { get; set; }

        [Display(Name = "Расширения")]
        public string Extensions { get; set; }
    }
}