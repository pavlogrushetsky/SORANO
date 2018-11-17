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

        public ReportController(IUserService userService, IExceptionService exceptionService, IReportService reportService) : base(userService, exceptionService)
        {
            _reportService = reportService;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult Report(string reportType, string from, string to)
        {
            var dateFrom = string.IsNullOrEmpty(from)
                ? (DateTime?)null
                : DateTime.ParseExact(from, "MM.yyyy", CultureInfo.InvariantCulture);

            var dateTo = string.IsNullOrEmpty(to)
                ? (DateTime?)null
                : DateTime.ParseExact(to, "MM.yyyy", CultureInfo.InvariantCulture);

            var report = _reportService.GetTurnoverReport(dateFrom, dateTo).Result;
            var lastDate = report.Items.Last().DateTimeString;
            var firstDate = report.Items.First().DateTimeString;

            var locationColumns = report.Items
                .SelectMany(i => i.LocationValues)
                .Select(l => new ReportColumn
                {
                    Value = l.LocationName
                })
                .Distinct()
                .ToList();

            var headerColumns = new List<ReportColumn>();
            headerColumns.Add(new ReportColumn { Value = "Месяц" });
            headerColumns.AddRange(locationColumns);
            headerColumns.Add(new ReportColumn { Value = "Всего" });

            var bodyRows = new List<ReportRow>();
            report.Items.ForEach(i =>
            {
                var row = new ReportRow
                {
                    Columns = new List<ReportColumn>()
                };

                row.Columns.Add(new ReportColumn { Value = i.DateTimeString });
                row.Columns.AddRange(locationColumns.Select(c => new ReportColumn
                {
                    Value = i.LocationValues
                        .SingleOrDefault(l => l.LocationName.Equals(c.Value))
                        ?.Value.ToString("0.00", new CultureInfo("ru-RU"))
                        ?? "0.00"
                }));
                row.Columns.Add(new ReportColumn { Value = i.Total.ToString("0.00", new CultureInfo("ru-RU")) });

                bodyRows.Add(row);
            });

            var turnoverReport = new TurnoverReport
            {
                Name = $"Отчёт по оборотам за период: {lastDate} - {firstDate}",
                Deliveries = new Report
                {
                    Header = new ReportHeader { Rows = new List<ReportRow> { new ReportRow { Columns = headerColumns }}},
                    Body = new ReportBody { Rows = bodyRows }
                },
                Sales = new Report
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
                },
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
    }
}