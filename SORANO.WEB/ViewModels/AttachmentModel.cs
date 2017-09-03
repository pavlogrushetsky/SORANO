using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels
{
    public class AttachmentModel : EntityBaseModel
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
        public string TypeID { get; set; }

        [Display(Name = "Тип")]
        public AttachmentTypeModel Type { get; set; }    
        
        public bool IsNew { get; set; }
    }
}
