using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Attachment;
using SORANO.WEB.ViewModels.Recommendation;

namespace SORANO.WEB.ViewModels.Common
{
    public class BaseDetailsViewModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Рекомендации")]
        public IList<RecommendationViewModel> Recommendations { get; set; }

        [Display(Name = "Вложения")]
        public IList<AttachmentViewModel> Attachments { get; set; }

        public string MainPicturePath { get; set; }

        public bool HasMainPicture => !string.IsNullOrEmpty(MainPicturePath);

        [Display(Name = "Создание")]
        public string Created { get; set; }

        [Display(Name = "Создание")]
        public string CreatedBy { get; set; }

        [Display(Name = "Изменение")]
        public string Modified { get; set; }

        [Display(Name = "Изменение")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Удаление")]
        public string Deleted { get; set; }

        [Display(Name = "Удаление")]
        public string DeletedBy { get; set; }

        [Display(Name = "Статус")]
        public bool IsDeleted { get; set; }
    }
}