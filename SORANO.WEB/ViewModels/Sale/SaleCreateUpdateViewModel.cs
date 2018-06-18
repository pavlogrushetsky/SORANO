using SORANO.WEB.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.Sale
{
    public class SaleCreateUpdateViewModel : BaseCreateUpdateViewModel
    {
        [Display(Name = "Место продажи *")]
        public int LocationID { get; set; }

        public string LocationName { get; set; }

        [Display(Name = "Клиент")]
        public int ClientID { get; set; }

        public string ClientName { get; set; }

        public string SelectedCurrency { get; set; } = "₴";

        [Display(Name = "Дата продажи")]
        public string Date { get; set; }

        public string DollarRate { get; set; }

        public string EuroRate { get; set; }

        [Display(Name = "Безналичный расчёт")]
        public bool IsCachless { get; set; }

        public bool IsSubmitted { get; set; }

        public bool AllowChangeLocation { get; set; } = true;

        public bool AllowCreation { get; set; }     
        
        public bool ShowSelected { get; set; }

        public string SearchCriteria { get; set; }
    }
}