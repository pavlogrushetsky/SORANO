using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;
using SORANO.BLL.Dtos.ReportDtos;
using SORANO.BLL.Services.Abstract;
using SORANO.WEB.Infrastructure.Filters;
using SORANO.WEB.ViewModels.Report;
// ReSharper disable UseObjectOrCollectionInitializer

namespace SORANO.WEB.Controllers
{
    [Authorize(Roles = "administrator,manager")]
    [CheckUser]
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;
        private readonly IHostingEnvironment _environment;
        private const string _dateFormat = "MM.yyyy";
        private readonly CultureInfo _culture = CultureInfo.InvariantCulture;
        private readonly CultureInfo _russianCulture = new CultureInfo("ru-RU");
        private const string _moneyFormat = "0.00";
        private const string _hryvna = "₴";

        public ReportController(IUserService userService, 
            IExceptionService exceptionService, 
            IReportService reportService,
            IHostingEnvironment environment) : base(userService, exceptionService)
        {
            _reportService = reportService;
            _environment = environment;
        }

        public IActionResult Index() => View();       

        [HttpPost]
        public IActionResult Report(string reportType, string from, string to, int? locationId, string locationName)
        {
            switch (reportType.ToLower())
            {
                case "обороты":
                    return PartialView("_TurnoverReport", CreateTurnoverReport(from, to));
                case "инвентаризация":
                    return PartialView("_InventoryReport", CreateInventoryReport(locationId, locationName));
                default:
                    return BadRequest();
            }
        }       
        public FileResult Export(string locationId, string locationName)
        {
            var id = string.IsNullOrEmpty(locationId) ? (int?)null : Convert.ToInt32(locationId);
            var report = _reportService.GetInventoryReport(id).Result;
            var excelPackage = report.Export("Инвентаризация");
            var fileName = $"Инвентаризация_{DateTime.Now.ToString("dd.MM.yyyy")}.xlsx";
            var tempFileName = $"inventory_{DateTime.Now:ddMMyyyHHmmss}.xlsx";
            var webRootPath = _environment.WebRootPath;
            var directory = Path.Combine(webRootPath, "reports");
            Directory.CreateDirectory(directory);
            var fullPath = $"{directory}\\{tempFileName}";

            var fileInfo = new FileInfo(fullPath);
            excelPackage.SaveAs(fileInfo);
            excelPackage.Dispose();

            var memory = new MemoryStream();
            using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            return File(memory, MimeTypeMap.GetMimeType(Path.GetExtension(fileName)), fileName);
        }

        private InventoryReport CreateInventoryReport(int? locationId, string locationName)
        {
            var reportDto = _reportService.GetInventoryReport(locationId).Result;

            var name = "Инвентаризация";
            if (!string.IsNullOrWhiteSpace(locationName))
                name += $" :: {locationName}";

            var reports = new Dictionary<string, Report>();

            if (reportDto.LocationGoods.Any())
            {
                foreach (var location in reportDto.LocationGoods.Keys)
                {
                    var headerColumns = new List<ReportColumn>();
                    headerColumns.Add(new ReportColumn { Value = "Артикул" });
                    headerColumns.Add(new ReportColumn { Value = "Код" });
                    headerColumns.Add(new ReportColumn { Value = "Тип" });
                    headerColumns.Add(new ReportColumn { Value = "Кол-во, шт." });

                    var bodyRows = new List<ReportRow>();
                    var goods = reportDto.LocationGoods[location];
                    foreach (var good in goods)
                    {
                        var row = new ReportRow
                        {
                            Columns = new List<ReportColumn>()
                        };

                        row.Columns.Add(new ReportColumn { Value = good.ArticleName });
                        row.Columns.Add(new ReportColumn { Value = good.ArticleCode });
                        row.Columns.Add(new ReportColumn { Value = good.ArticleType });
                        row.Columns.Add(new ReportColumn { Value = good.Quantity.ToString() });

                        bodyRows.Add(row);
                    }

                    var report = new Report
                    {
                        Header = new ReportHeader { Rows = new List<ReportRow> { new ReportRow { Columns = headerColumns } } },
                        Body = new ReportBody { Rows = bodyRows }
                    };
                    reports.Add(location, report);
                }
            }
            else
            {
                var report = new Report
                {
                    Header = new ReportHeader { Rows = new List<ReportRow>() },
                    Body = new ReportBody
                    {
                        Rows = new List<ReportRow>
                        {
                            new ReportRow{Columns = new List<ReportColumn>{new ReportColumn { Value = "Для указанного места товары отсутствуют."} }}
                        }
                    }
                };
                reports.Add("no-data", report);
            }

            return new InventoryReport
            {
                Name = name,
                LocationReports = reports
            };
        }

        private TurnoverReport CreateTurnoverReport(string from, string to)
        {
            var dateFrom = ParseDateTime(from);
            var dateTo = ParseDateTime(to);

            var report = _reportService.GetTurnoverReport(dateFrom, dateTo).Result;

            var name = "Отчёт по оборотам";
            if (dateFrom.HasValue && !dateTo.HasValue || dateFrom.HasValue && dateFrom.Value > dateTo.Value)
                name = name + $" за период c: {MonthYearString(dateFrom.Value)}";
            else if (dateTo.HasValue && !dateFrom.HasValue)
                name = name + $" за период по: {MonthYearString(dateTo.Value)}";
            else if (dateFrom.HasValue)
                name = name + $" за период: {MonthYearString(dateFrom.Value)} - {MonthYearString(dateTo.Value)}";

            var deliveriesReport = new Report();

            if (report.Deliveries.Any())
            {
                var locationColumns = report.Deliveries
                    .Select(d => d.Value.LocationDeliveries)
                    .SelectMany(d => d.Keys)
                    .Distinct()
                    .OrderBy(l => l)
                    .Select(l => new ReportColumn { Value = l })
                    .ToList();

                var maxMonthValue = report.Deliveries
                    .Select(d => d.Value)
                    .SelectMany(d => d.LocationDeliveries.Values)
                    .Max()
                    .ToString(_moneyFormat, _russianCulture);

                var minMonthValue = report.Deliveries
                    .Select(d => d.Value)
                    .SelectMany(d => d.LocationDeliveries.Values)
                    .Min()
                    .ToString(_moneyFormat, _russianCulture);

                var maxTotalValue = report.Deliveries
                    .Select(d => d.Value.Total)
                    .Max()
                    .ToString(_moneyFormat, _russianCulture);

                var minTotalValue = report.Deliveries
                    .Select(d => d.Value.Total)
                    .Min()
                    .ToString(_moneyFormat, _russianCulture);

                var headerColumns = new List<ReportColumn>();
                headerColumns.Add(new ReportColumn { Value = "Месяц" });
                headerColumns.AddRange(locationColumns);
                headerColumns.Add(new ReportColumn { Value = "Всего" });

                var bodyRows = new List<ReportRow>();
                foreach (var delivery in report.Deliveries)
                {
                    var row = new ReportRow
                    {
                        Columns = new List<ReportColumn>()
                    };

                    row.Columns.Add(new ReportColumn { Value = MonthYearString(delivery.Key) });
                    row.Columns.AddRange(locationColumns.Select(c =>
                    {
                        var value = delivery.Value.LocationDeliveries.ContainsKey(c.Value)
                            ? delivery.Value.LocationDeliveries[c.Value].ToString(_moneyFormat, _russianCulture)
                            : string.Empty;

                        var isMax = value.Equals(maxMonthValue);
                        var isMin = value.Equals(minMonthValue);

                        return new ReportColumn
                        {
                            Value = string.IsNullOrEmpty(value)
                                ? $"{_moneyFormat} {_hryvna}"
                                : value + $" {_hryvna}",
                            IsMax = isMax,
                            IsMin = isMin
                        };
                    }));

                    var total = delivery.Value.Total.ToString(_moneyFormat, _russianCulture);
                    row.Columns.Add(new ReportColumn
                    {
                        Value = $"{total} {_hryvna}",
                        IsMax = total.Equals(maxTotalValue),
                        IsMin = total.Equals(minTotalValue)
                    });

                    bodyRows.Add(row);
                }

                deliveriesReport.Header = new ReportHeader { Rows = new List<ReportRow> { new ReportRow { Columns = headerColumns } } };
                deliveriesReport.Body = new ReportBody { Rows = bodyRows };
            }
            else
            {
                deliveriesReport.Header = new ReportHeader { Rows = new List<ReportRow>() };
                deliveriesReport.Body = new ReportBody
                {
                    Rows = new List<ReportRow>
                {
                    new ReportRow{Columns = new List<ReportColumn>{new ReportColumn { Value = "За указанный период данные по поставкам отсутствуют."} }}
                }
                };
            }

            return new TurnoverReport
            {
                Name = name,
                Deliveries = deliveriesReport,
                Sales = CreateSalesReport(report.Sales, false, false),
                Writeoffs = CreateSalesReport(report.Writeoffs, true, false),
                Profit = CreateSalesReport(report.Profit, false, true)
            };
        }

        private Report CreateSalesReport(Dictionary<DateTime, LocationSalesDto> sales, bool isWriteOff, bool isProfit)
        {
            var report = new Report();

            if (sales.Any())
            {
                var locationColumns = sales
                    .Select(d => d.Value.LocationSales)
                    .SelectMany(d => d.Keys)
                    .Distinct()
                    .OrderBy(l => l)
                    .Select(l => new ReportColumn {Value = l})
                    .ToList();

                var maxMonthValue = sales
                    .Select(d => d.Value)
                    .SelectMany(d => d.LocationSales.Values.Select(v => v.Cash + v.Cashless))
                    .DefaultIfEmpty(decimal.MaxValue)
                    .Max();

                var minMonthValue = sales
                    .Select(d => d.Value)
                    .SelectMany(d => d.LocationSales.Values.Select(v => v.Cash + v.Cashless))
                    .DefaultIfEmpty(decimal.MaxValue)
                    .Min();

                var maxTotalValue = sales
                    .Select(d => d.Value.Total)
                    .DefaultIfEmpty(decimal.MaxValue)
                    .Max();

                var minTotalValue = sales
                    .Select(d => d.Value.Total)
                    .DefaultIfEmpty(decimal.MaxValue)
                    .Min();

                var maxCashTotalValue = sales
                    .Select(d => d.Value.CashTotal)
                    .Where(v => v > 0M)
                    .DefaultIfEmpty(decimal.MaxValue)
                    .Max();

                var minCashTotalValue = sales
                    .Select(d => d.Value.CashTotal)
                    .Where(v => v > 0M)
                    .DefaultIfEmpty(decimal.MaxValue)
                    .Min();

                var maxCashlessTotalValue = sales
                    .Select(d => d.Value.CashlessTotal)
                    .Where(v => v > 0M)
                    .DefaultIfEmpty(decimal.MaxValue)
                    .Max();

                var minCashlessTotalValue = sales
                    .Select(d => d.Value.CashlessTotal)
                    .Where(v => v > 0M)
                    .DefaultIfEmpty(decimal.MaxValue)
                    .Min();

                var headerColumns = new List<ReportColumn>();
                headerColumns.Add(new ReportColumn {Value = "Месяц"});
                headerColumns.AddRange(locationColumns);
                headerColumns.Add(new ReportColumn {Value = "Всего"});
                if (!isProfit && !isWriteOff)
                    headerColumns.AddRange(new List<ReportColumn>
                    {
                        new ReportColumn {Value = "Всего, нал."},
                        new ReportColumn {Value = "Всего, безнал."},
                    });

                var bodyRows = new List<ReportRow>();
                foreach (var sale in sales)
                {
                    var row = new ReportRow
                    {
                        Columns = new List<ReportColumn>()
                    };

                    row.Columns.Add(new ReportColumn {Value = MonthYearString(sale.Key)});
                    row.Columns.AddRange(locationColumns.Select(c =>
                    {
                        var cash = sale.Value.LocationSales.ContainsKey(c.Value)
                            ? sale.Value.LocationSales[c.Value].Cash
                            : 0.0M;

                        var cashless = sale.Value.LocationSales.ContainsKey(c.Value)
                            ? sale.Value.LocationSales[c.Value].Cashless
                            : 0.0M;

                        var cashStr = $"{cash.ToString(_moneyFormat, _russianCulture)} {_hryvna}";
                        var cashlessStr = $"{cashless.ToString(_moneyFormat, _russianCulture)} {_hryvna}";

                        var saleTotal = cash + cashless;

                        var saleTotalStr = $"{saleTotal.ToString(_moneyFormat, _russianCulture)} {_hryvna}";

                        var isMax = saleTotal.Equals(maxMonthValue);
                        var isMin = saleTotal.Equals(minMonthValue);

                        return new ReportColumn
                        {
                            Value = isWriteOff || isProfit ? saleTotalStr : $"{cashStr} / {cashlessStr}",
                            IsMax = isMax,
                            IsMin = isMin,
                            IsNegative = saleTotal < 0.0M
                        };
                    }));

                    var total = sale.Value.Total;
                    row.Columns.Add(new ReportColumn
                    {
                        Value = $"{total.ToString(_moneyFormat, _russianCulture)} {_hryvna}",
                        IsMax = total.Equals(maxTotalValue),
                        IsMin = total.Equals(minTotalValue),
                        IsNegative = total < 0.0M
                    });

                    if (!isProfit && !isWriteOff)
                    {
                        var cashTotal = sale.Value.LocationSales.Sum(s => s.Value.Cash);
                        var cashlessTotal = sale.Value.LocationSales.Sum(s => s.Value.Cashless);                        

                        row.Columns.AddRange(new List<ReportColumn>
                        {
                            new ReportColumn 
                            { 
                                Value = $"{cashTotal.ToString(_moneyFormat, _russianCulture)}",
                                IsMax = cashTotal.Equals(maxCashTotalValue),
                                IsMin = cashTotal.Equals(minCashTotalValue),
                            },
                            new ReportColumn 
                            {
                                Value = $"{cashlessTotal.ToString(_moneyFormat, _russianCulture)}",
                                IsMax = cashlessTotal.Equals(maxCashlessTotalValue),
                                IsMin = cashlessTotal.Equals(minCashlessTotalValue),
                            },
                        });
                    }

                    bodyRows.Add(row);
                }

                report.Header = new ReportHeader {Rows = new List<ReportRow> {new ReportRow {Columns = headerColumns}}};
                report.Body = new ReportBody {Rows = bodyRows};
            }
            else
            {
                report.Header = new ReportHeader {Rows = new List<ReportRow>()};
                report.Body = new ReportBody
                {
                    Rows = new List<ReportRow>
                    {
                        new ReportRow
                        {
                            Columns = new List<ReportColumn>
                            {
                                new ReportColumn {Value = $"За указанный период данные по {(isWriteOff ? "списаниям" : isProfit ? "прибыли" : "продажам")} отсутствуют."}
                            }
                        }
                    }
                };
            }

            return report;
        }

        private static string MonthYearString(DateTime dateTime)
        {
            var month = "";
            switch (dateTime.Month)
            {
                case 1:
                    month = "Январь";
                    break;
                case 2:
                    month = "Февраль";
                    break;
                case 3:
                    month = "Март";
                    break;
                case 4:
                    month = "Апрель";
                    break;
                case 5:
                    month = "Май";
                    break;
                case 6:
                    month = "Июнь";
                    break;
                case 7:
                    month = "Июль";
                    break;
                case 8:
                    month = "Август";
                    break;
                case 9:
                    month = "Сентябрь";
                    break;
                case 10:
                    month = "Октябрь";
                    break;
                case 11:
                    month = "Ноябрь";
                    break;
                case 12:
                    month = "Декабрь";
                    break;
            }

            return $"{month} {dateTime.Year}";
        }

        private DateTime? ParseDateTime(string dateTime) => string.IsNullOrEmpty(dateTime)
            ? (DateTime?) null
            : DateTime.ParseExact(dateTime, _dateFormat, _culture);
    }
}