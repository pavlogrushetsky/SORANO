using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SORANO.WEB.Models.Recommendation;

namespace SORANO.WEB.Models
{
    public abstract class EntityBaseModel
    {
        [Display(Name = "ID")]
        public virtual int ID { get; set; }

        [Display(Name = "Рекомендации")]
        public virtual List<RecommendationModel> Recommendations { get; set; } = new List<RecommendationModel>();

        [Display(Name = "Создание")]
        public virtual string Created { get; set; }

        public virtual string CreatedBy { get; set; }

        [Display(Name = "Последнее изменение")]
        public virtual string Modified { get; set; }

        public virtual string ModifiedBy { get; set; }

        public virtual bool CanBeDeleted { get; set; }
    }
}