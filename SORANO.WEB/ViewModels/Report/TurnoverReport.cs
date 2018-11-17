using System;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.Report
{
    public class TurnoverReport
    {
        public string Name { get; set; }

        [Display(Name = "Сгенерирован")]
        public string Generated { get; set; } = DateTime.Now.ToString("dd.MM.yyyy");

        [Display(Name = "Поставки")]
        public Report Deliveries { get; set; }

        [Display(Name = "Продажи")]
        public Report Sales { get; set; }

        [Display(Name = "Прибыль")]
        public Report Profit { get; set; }
    }
}