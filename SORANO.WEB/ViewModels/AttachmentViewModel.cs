using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels
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
        
        public bool IsNew { get; set; }
    }
}
