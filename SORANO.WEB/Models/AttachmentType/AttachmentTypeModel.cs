using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models.AttachmentType
{
    public class AttachmentTypeModel : EntityBaseModel
    {
        [Required(ErrorMessage = "Необходимо указать название типа вложений")]
        [MaxLength(200, ErrorMessage = "Длина названия не должна превышать 200 символов")]
        [MinLength(5, ErrorMessage = "Длина названия должна содержать не менее 5 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Длина описания не должна превышать 1000 символов")]
        [Display(Name = "Описание")]
        public string Comment { get; set; }

        [Display(Name = "Вложений")]
        public int AttachmentsCount { get; set; }
    }
}
