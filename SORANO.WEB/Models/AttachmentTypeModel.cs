using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class AttachmentTypeModel : EntityBaseModel
    {
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Коментарий")]
        public string Comment { get; set; }

        [Display(Name = "Расширения")]
        public string Extensions { get; set; }

        public string MimeTypes { get; set; }

        [Display(Name = "Вложений")]
        public int AttachmentsCount { get; set; }
    }
}
