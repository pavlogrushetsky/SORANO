using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.AttachmentType
{
    public class AttachmentTypeDeleteViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Коментарий")]
        public string Comment { get; set; }

        public bool CanBeDeleted { get; set; }
    }
}