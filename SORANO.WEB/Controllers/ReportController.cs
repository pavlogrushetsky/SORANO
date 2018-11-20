using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

            var salesReport = new Report();

            if (report.Sales.Any())
            {
                var locationColumns = report.Sales
                    .Select(d => d.Value.LocationSales)
                    .SelectMany(d => d.Keys)
                    .Distinct()
                    .OrderBy(l => l)
                    .Select(l => new ReportColumn { Value = l })
                    .ToList();

                var maxMonthValue = report.Sales
                    .Select(d => d.Value)
                    .SelectMany(d => d.LocationSales.Values)
                    .Max()
                    .ToString(_moneyFormat, _russianCulture);

                var minMonthValue = report.Sales
                    .Select(d => d.Value)
                    .SelectMany(d => d.LocationSales.Values)
                    .Min()
                    .ToString(_moneyFormat, _russianCulture);

                var maxTotalValue = report.Sales
                    .Select(d => d.Value.Total)
                    .Max()
                    .ToString(_moneyFormat, _russianCulture);

                var minTotalValue = report.Sales
                    .Select(d => d.Value.Total)
                    .Min()
                    .ToString(_moneyFormat, _russianCulture);

                var headerColumns = new List<ReportColumn>();
                headerColumns.Add(new ReportColumn { Value = "Месяц" });
                headerColumns.AddRange(locationColumns);
                headerColumns.Add(new ReportColumn { Value = "Всего" });

                var bodyRows = new List<ReportRow>();
                foreach (var sale in report.Sales)
                {
                    var row = new ReportRow
                    {
                        Columns = new List<ReportColumn>()
                    };

                    row.Columns.Add(new ReportColumn { Value = MonthYearString(sale.Key) });
                    row.Columns.AddRange(locationColumns.Select(c =>
                    {
                        var value = sale.Value.LocationSales.ContainsKey(c.Value)
                            ? sale.Value.LocationSales[c.Value].ToString(_moneyFormat, _russianCulture)
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

                    var total = sale.Value.Total.ToString(_moneyFormat, _russianCulture);
                    row.Columns.Add(new ReportColumn
                    {
                        Value = $"{total} {_hryvna}",
                        IsMax = total.Equals(maxTotalValue),
                        IsMin = total.Equals(minTotalValue)
                    });

                    bodyRows.Add(row);
                }

                salesReport.Header = new ReportHeader { Rows = new List<ReportRow> { new ReportRow { Columns = headerColumns } } };
                salesReport.Body = new ReportBody { Rows = bodyRows };
            }
            else
            {
                salesReport.Header = new ReportHeader { Rows = new List<ReportRow>() };
                salesReport.Body = new ReportBody
                {
                    Rows = new List<ReportRow>
                    {
                        new ReportRow{Columns = new List<ReportColumn>{new ReportColumn { Value = "За указанный период данные по продажам отсутствуют."} }}
                    }
                };
            }



            var turnoverReport = new TurnoverReport
            {
                Name = name,
                Deliveries = deliveriesReport,
                Sales = salesReport,
                Profit = new Report
                {
                    Header = new ReportHeader
                    {
                        Rows = new List<ReportRow>
                        {
                            new ReportRow
                            {
                                Columns = new List<ReportColumn>
                                {
                                    new ReportColumn {Value = "Месяц"},
                                    new ReportColumn {Value = "Магазин №1"},
                                    new ReportColumn {Value = "Магазин №2"},
                                    new ReportColumn {Value = "Магазин №3"},
                                    new ReportColumn {Value = "Всего"}
                                }
                            }
                        }
                    },
                    Body = new ReportBody
                    {
                        Rows = new List<ReportRow>
                        {
                            new ReportRow
                            {
                                Columns = new List<ReportColumn>
                                {
                                    new ReportColumn {Value = "Июль 2018"},
                                    new ReportColumn {Value = "67"},
                                    new ReportColumn {Value = "567"},
                                    new ReportColumn {Value = "12345"},
                                    new ReportColumn {Value = "65847"}
                                }
                            },
                            new ReportRow
                            {
                                Columns = new List<ReportColumn>
                                {
                                    new ReportColumn {Value = "Июль 2018"},
                                    new ReportColumn {Value = "67"},
                                    new ReportColumn {Value = "567"},
                                    new ReportColumn {Value = "12345"},
                                    new ReportColumn {Value = "65847"}
                                }
                            },
                            new ReportRow
                            {
                                Columns = new List<ReportColumn>
                                {
                                    new ReportColumn {Value = "Июль 2018"},
                                    new ReportColumn {Value = "67"},
                                    new ReportColumn {Value = "567"},
                                    new ReportColumn {Value = "12345"},
                                    new ReportColumn {Value = "65847"}
                                }
                            }
                        }
                    }
                }
            };

            return PartialView("_TurnoverReport", turnoverReport);
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