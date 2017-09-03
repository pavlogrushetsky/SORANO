using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels
{
    public class LocationModel : EntityBaseModel
    {
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Коментарий")]
        public string Comment { get; set; }

        [Display(Name = "Тип")]
        public string TypeID { get; set; }

        [Display(Name = "Тип")]
        public LocationTypeModel Type { get; set; }

        [Display(Name = "Товары")]
        public List<StoredGoodsModel> Goods { get; set; }

        public string ReturnPath { get; set; }
    }
}
