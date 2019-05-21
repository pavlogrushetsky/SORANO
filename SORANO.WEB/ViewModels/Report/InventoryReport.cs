using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SORANO.WEB.ViewModels.Report
{
    public class InventoryReport
    {
        public string Name { get; set; }

        [Display(Name = "Сгенерирован")]
        public string Generated { get; set; } = DateTime.Now.ToString("dd.MM.yyyy");

        public Dictionary<string, Report> LocationReports { get; set; }
    }

    public static class InventoryReportExtensions
    {
        public static ExcelPackage Export(this InventoryReport inventoryReport)
        {
            var excelPackage = new ExcelPackage();
            for (int i = 0; i < inventoryReport.LocationReports.Count; i++)
            {
                var report = inventoryReport.LocationReports.ElementAt(i);
                excelPackage.Workbook.Worksheets.Add(report.Key);
            }

            return excelPackage;
        }
    }
}