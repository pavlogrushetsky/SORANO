using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels
{
    public class DetailsViewModel
    {
        [Display(Name = "Создание")]
        public virtual string Created { get; set; }

        public virtual string CreatedBy { get; set; }

        [Display(Name = "Изменение")]
        public virtual string Modified { get; set; }

        public virtual string ModifiedBy { get; set; }

        [Display(Name = "Удаление")]
        public virtual string Deleted { get; set; }

        public virtual string DeletedBy { get; set; }

        public virtual bool CanBeDeleted { get; set; }

        [Display(Name = "Статус")]
        public virtual bool IsDeleted { get; set; }
    }
}