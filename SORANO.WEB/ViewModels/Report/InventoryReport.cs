using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SORANO.WEB.ViewModels.Report
{
    public class InventoryReport
    {
        public string Name { get; set; }

        [Display(Name = "Сгенерирован")]
        public string Generated { get; set; } = DateTime.Now.ToString("dd.MM.yyyy");

        public Dictionary<string, Report> LocationReports { get; set; }
    }
}