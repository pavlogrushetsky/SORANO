using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels
{
    public class ClientModel : EntityBaseModel
    {
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Номер карты")]
        public string CardNumber { get; set; }

        [Display(Name = "Покупки")]
        public List<SaleModel> Purchases { get; set; } = new List<SaleModel>();
    }
}