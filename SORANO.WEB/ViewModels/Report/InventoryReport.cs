using OfficeOpenXml;
using OfficeOpenXml.Style;
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
                var worksheet = excelPackage.Workbook.Worksheets.Add(report.Key);
                var headerRow = report.Value.Header.Rows.Select(r => r.Columns.Select(c => c.Value).ToArray()).ToList();
                var headerRange = $"A1:{char.ConvertFromUtf32(headerRow[0].Length + 64)}1";
                worksheet.Cells[headerRange].LoadFromArrays(headerRow);
                worksheet.Cells[headerRange].Style.Font.Bold = true;
                worksheet.Cells[headerRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[headerRange].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);                
                var cellsData = report.Value.Body.Rows.Select(r => r.Columns.Select(c => c.Value).ToArray()).ToList();
                worksheet.Cells[2, 1].LoadFromArrays(cellsData);
                var totalRange = $"A1:{char.ConvertFromUtf32(headerRow[0].Length + 64)}{report.Value.Body.Rows.Count + 1}";
                worksheet.Cells[totalRange].AutoFitColumns();
            }

            return excelPackage;
        }
    }
}