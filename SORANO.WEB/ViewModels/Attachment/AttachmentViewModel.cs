using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.Attachment
{
    public class AttachmentViewModel
    {
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Путь")]
        public string FullPath { get; set; }

        [Display(Name = "Расширение")]
        public string Extension { get; set; }

        [Display(Name = "Тип")]
        public int TypeID { get; set; }

        [Display(Name = "Тип")]
        public string TypeName { get; set; }

        public string MimeTypes { get; set; }
        
        public bool IsNew { get; set; }
    }
}
