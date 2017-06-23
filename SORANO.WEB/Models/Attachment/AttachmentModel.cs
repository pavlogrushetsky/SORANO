using SORANO.WEB.Models.AttachmentType;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models.Attachment
{
    public class AttachmentModel : EntityBaseModel
    {
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Необходимо указать название файла")]
        [MaxLength(255, ErrorMessage = "Длина названия файла не должна превышать 255 символов")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Путь")]
        [Required(ErrorMessage = "Необходимо указать путь к файлу")]
        [MaxLength(1000, ErrorMessage = "Длина пути к файлу не должна превышать 1000 символов")]
        public string FullPath { get; set; }

        [Display(Name = "Расширение")]
        public string Extension { get; set; }

        [Display(Name = "Тип")]
        public string TypeID { get; set; }

        [Display(Name = "Тип")]
        public AttachmentTypeModel Type { get; set; }
    }
}
