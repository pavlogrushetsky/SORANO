using OfficeOpenXml;
using OfficeOpenXml.Style;
using SORANO.BLL.Dtos.ReportDtos;
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
            for (var i = 0; i < inventoryReport.LocationReports.Count; i++)
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

        public static ExcelPackage Export(this InventoryReportDto inventoryReport, string worksheetName)
        {
            var excelPackage = new ExcelPackage();
            var worksheet = excelPackage.Workbook.Worksheets.Add(worksheetName);

            var headerRows = new List<string[]>
            {
                new string[] { "Код", "Наименование", "Кол-во, шт." }
            };

            var locations = inventoryReport.LocationGoods.Keys.OrderBy(x => x).ToList();
            var multipleLocations = locations.Count > 1;
            if (multipleLocations)
            {                
                var headerRow = new List<string> { "", "", "Всего" };
                locations.ForEach(l => headerRow.Add(l));
                headerRows.Add(headerRow.ToArray());
            }

            var rangeEndLetter = multipleLocations 
                ? char.ConvertFromUtf32(headerRows[1].Length + 64) 
                : "C";

            var headerRange = $"A1:{rangeEndLetter}{(multipleLocations ? "2" : "1")}";

            worksheet.Cells[headerRange].LoadFromArrays(headerRows);            

            worksheet.Cells[headerRange].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var totalRow = new List<string> { "Итого:", "" };
            if (!multipleLocations)
            {
                var total = inventoryReport.LocationGoods.FirstOrDefault().Value.Sum(x => x.Quantity);
                totalRow.Add(total.ToString());
            }
            else
            {
                var totals = inventoryReport.LocationGoods
                    .OrderBy(x => x.Key)
                    .Select(x => x.Value.Sum(y => y.Quantity))
                    .ToList();

                totalRow.Add(totals.Sum().ToString());
                totals.ForEach(x => totalRow.Add(x.ToString()));
            }

            var currentRow = multipleLocations ? 3 : 2;

            worksheet.Cells[currentRow, 1].LoadFromArrays(new List<string[]> { totalRow.ToArray() });     

            currentRow += 1;

            var articleTypes = inventoryReport.LocationGoods
                .SelectMany(x => x.Value)
                .GroupBy(x => x.ArticleType)
                .Select(x => x.Key)
                .OrderBy(x => x)
                .ToList();            

            articleTypes.ForEach(type => 
            {
                worksheet.Cells[currentRow, 1].LoadFromArrays(new List<string[]> { new string[] { type } });
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                var rowRange = $"A{currentRow}:{rangeEndLetter}{currentRow}";
                worksheet.Cells[rowRange].Merge = true;
                worksheet.Cells[rowRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowRange].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(177, 213, 227));

                currentRow += 1;

                var articles = inventoryReport.LocationGoods
                    .SelectMany(x => x.Value)
                    .Where(x => x.ArticleType.Equals(type))
                    .GroupBy(x => new { x.ArticleCode, x.ArticleName })
                    .Select(x => x.Key)
                    .OrderBy(x => x.ArticleName)
                    .ToList();

                articles.ForEach(article =>
                {
                    var result = inventoryReport.LocationGoods
                        .OrderBy(x => x.Key)
                        .Select(x => x.Value.FirstOrDefault(v => v.ArticleName.Equals(article.ArticleName))?.Quantity ?? (int?)null)
                        .ToList();

                    var cells = new List<string> { article.ArticleCode, article.ArticleName };
                    if (!multipleLocations)
                    {
                        cells.Add(result.FirstOrDefault()?.ToString() ?? string.Empty);
                    }
                    else
                    {
                        cells.Add(result.Sum().ToString());
                        result.ForEach(r => cells.Add(r.ToString()));
                    }

                    worksheet.Cells[currentRow, 1].LoadFromArrays(new List<string[]> { cells.ToArray() });

                    currentRow += 1;
                });               
            });

            var totalRange = $"A1:{rangeEndLetter}{currentRow - 1}";

            worksheet.Cells[totalRange].AutoFitColumns();            

            if (multipleLocations)
            {
                worksheet.Cells["A1:A2"].Merge = true;
                worksheet.Cells["B1:B2"].Merge = true;
                worksheet.Cells[$"C1:{rangeEndLetter}1"].Merge = true;
            }

            var totalRowNumber = multipleLocations ? 3 : 2;
            var totalRowRange = $"A{totalRowNumber}:B{totalRowNumber}";
            worksheet.Cells[totalRowRange].Merge = true;
            worksheet.Cells[totalRowRange].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[$"A{totalRowNumber}:{rangeEndLetter}{totalRowNumber}"].Style.Font.Italic = true;

            worksheet.Cells[totalRange].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[totalRange].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[totalRange].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[totalRange].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            return excelPackage;
        }
    }
}