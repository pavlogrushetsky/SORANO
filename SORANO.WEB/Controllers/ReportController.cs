using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private const string _dateFormat = "MM.yyyy";
        private readonly CultureInfo _culture = CultureInfo.InvariantCulture;
        private readonly CultureInfo _russianCulture = new CultureInfo("ru-RU");
        private const string _moneyFormat = "0.00";
        private const string _hryvna = "₴";

        public ReportController(IUserService userService, IExceptionService exceptionService, IReportService reportService) : base(userService, exceptionService)
        {
            _reportService = reportService;
        }

        public IActionResult Index() => View();       

        [HttpPost]
        public IActionResult Report(string reportType, string from, string to)
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
                headerColumns.Add(new ReportColumn {Value = "Месяц"});
                headerColumns.AddRange(locationColumns);
                headerColumns.Add(new ReportColumn {Value = "Всего"});

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

                deliveriesReport.Header = new ReportHeader {Rows = new List<ReportRow> {new ReportRow {Columns = headerColumns}}};
                deliveriesReport.Body = new ReportBody {Rows = bodyRows};
            }
            else
            {
                deliveriesReport.Header = new ReportHeader{Rows = new List<ReportRow>()};
                deliveriesReport.Body = new ReportBody{Rows = new List<ReportRow>
                {
                    new ReportRow{Columns = new List<ReportColumn>{new ReportColumn { Value = "За указанный период данные по поставкам отсутствуют."} }}
                }};
            }

            var turnoverReport = new TurnoverReport
            {
                Name = name,
                Deliveries = deliveriesReport,
                Sales = CreateSalesReport(report.Sales, false, false),
                Writeoffs = CreateSalesReport(report.Writeoffs, true, false),
                Profit = CreateSalesReport(report.Profit, false, true)
            };

            return PartialView("_TurnoverReport", turnoverReport);
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
                    .SelectMany(d => d.LocationSales.Values)
                    .Max();

                var minMonthValue = sales
                    .Select(d => d.Value)
                    .SelectMany(d => d.LocationSales.Values)
                    .Min();

                var maxTotalValue = sales
                    .Select(d => d.Value.Total)
                    .Max();

                var minTotalValue = sales
                    .Select(d => d.Value.Total)
                    .Min();

                var headerColumns = new List<ReportColumn>();
                headerColumns.Add(new ReportColumn {Value = "Месяц"});
                headerColumns.AddRange(locationColumns);
                headerColumns.Add(new ReportColumn {Value = "Всего"});

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
                        var value = sale.Value.LocationSales.ContainsKey(c.Value)
                            ? sale.Value.LocationSales[c.Value]
                            : 0.0M;

                        var valueStr = $"{value.ToString(_moneyFormat, _russianCulture)} {_hryvna}";

                        var isMax = value.Equals(maxMonthValue);
                        var isMin = value.Equals(minMonthValue);

                        return new ReportColumn
                        {
                            Value = valueStr,
                            IsMax = isMax,
                            IsMin = isMin,
                            IsNegative = value < 0.0M
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