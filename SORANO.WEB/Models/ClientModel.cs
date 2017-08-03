using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
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
    }
}