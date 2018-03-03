using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.Sale
{
    public class SaleDetailsViewModel : BaseDetailsViewModel
    {
        [Display(Name = "Валюта")]
        public string Currency { get; set; }

        [Display(Name = "Дата продажи")]
        public string Date { get; set; }

        [Display(Name = "Курс доллара")]
        public string DollarRate { get; set; }

        [Display(Name = "Курс евро")]
        public string EuroRate { get; set; }

        public int LocationId { get; set; }

        [Display(Name = "Место продажи")]
        public string LocationName { get; set; }

        public int? ClientId { get; set; }

        [Display(Name = "Клиент")]
        public string ClientName { get; set; }

        [Display(Name = "Общая сумма")]
        public string TotalPrice { get; set; }

        public int UserId { get; set; }

        [Display(Name = "Продавец")]
        public string UserName { get; set; }

        [Display(Name = "Статус")]
        public bool IsSubmitted { get; set; }

        [Display(Name = "Товары")]
        public List<SaleItemDetailsViewModel> Items = new List<SaleItemDetailsViewModel>();
    }
}