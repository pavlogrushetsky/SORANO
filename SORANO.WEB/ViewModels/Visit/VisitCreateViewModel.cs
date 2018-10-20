using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.Visit
{
    public class VisitCreateViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Код посетителей:")]
        public string Code { get; set; }

        [Display(Name = "Дата посещения:")]
        public string Date { get; set; }

        [Display(Name = "Место:")]
        public int LocationID { get; set; }

        public string LocationName { get; set; }
    }
}