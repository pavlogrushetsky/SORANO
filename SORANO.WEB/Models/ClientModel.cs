using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.Models
{
    public class ClientModel : EntityBaseModel
    {
        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Необходимо указать имя клиента")]
        [MaxLength(200, ErrorMessage = "Длина имени не должна превышать 200 символов")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        [MaxLength(1000, ErrorMessage = "Длина описания не должна превышать 1000 символов")]
        public string Description { get; set; }

        [Display(Name = "Номер телефона")]
        [MaxLength(200, ErrorMessage = "Длина номера телефона не должна превышать 200 символов")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Номер карты")]
        [MaxLength(200, ErrorMessage = "Длина номера карты не должна превышать 200 символов")]
        public string CardNumber { get; set; }
    }
}