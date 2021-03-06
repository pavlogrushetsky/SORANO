﻿using System.ComponentModel.DataAnnotations;
using SORANO.WEB.ViewModels.Common;

namespace SORANO.WEB.ViewModels.Delivery
{
    public class DeliveryCreateUpdateViewModel : BaseCreateUpdateViewModel
    {
        [Display(Name = "Номер накладной *")]
        public string BillNumber { get; set; }

        public string TotalGrossPrice { get; set; }

        public string TotalDiscount { get; set; }

        public string TotalDiscountedPrice { get; set; }

        public string SelectedCurrency { get; set; } = "₴";

        [Display(Name = "Поставщик *")]
        public int SupplierID { get; set; }

        public string SupplierName { get; set; }

        [Display(Name = "Место поставки *")]
        public int LocationID { get; set; }

        public string LocationName { get; set; }

        [Display(Name = "Дата поставки *")]
        public string DeliveryDate { get; set; }

        [Display(Name = "Дата оплаты")]
        public string PaymentDate { get; set; }

        public string DollarRate { get; set; }

        public string EuroRate { get; set; }

        public bool IsSubmitted { get; set; }

        public bool AllowChangeLocation { get; set; }

        public bool AllowCreation { get; set; }

        public int ItemsCount { get; set; }
    }
}