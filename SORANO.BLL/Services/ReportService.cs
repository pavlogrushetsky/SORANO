using System;
using System.Collections.Generic;
using System.Linq;
using SORANO.BLL.Dtos.ReportDtos;
using SORANO.BLL.Services.Abstract;
using SORANO.CORE.StockEntities;
using SORANO.DAL.Repositories;

namespace SORANO.BLL.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ServiceResponse<TurnoverReportDto> GetTurnoverReport(DateTime? from, DateTime? to)
        {
            var deliveries = _unitOfWork.Get<Delivery>()
                .GetAll(d => (!from.HasValue ||
                              d.DeliveryDate.Year >= from.Value.Year && d.DeliveryDate.Month >= from.Value.Month) &&
                             (!to.HasValue ||
                              from.HasValue && to.Value < from.Value ||
                              d.DeliveryDate.Year <= to.Value.Year && d.DeliveryDate.Month <= to.Value.Month), 
                    d => d.DeliveryLocation)
                .OrderByDescending(d => d.DeliveryDate)
                .ToList()
                .GroupBy(d => new {d.DeliveryDate.Year, d.DeliveryDate.Month})
                .Select(d =>
                {
                    var monthKey = new DateTime(d.Key.Year, d.Key.Month, 1);
                    var locationDeliveries = new LocationDeliveriesDto
                    {
                        Total = d.AsEnumerable().Sum(x => x.DollarRate.HasValue
                            ? x.TotalGrossPrice * x.DollarRate.Value
                            : x.EuroRate.HasValue
                                ? x.TotalGrossPrice * x.EuroRate.Value
                                : x.TotalGrossPrice),
                        LocationDeliveries = d.GroupBy(i => i.DeliveryLocation).Select(i =>
                        {
                            var location = i.Key.Name;
                            var value = i.AsEnumerable().Sum(x => x.DollarRate.HasValue
                                ? x.TotalGrossPrice * x.DollarRate.Value
                                : x.EuroRate.HasValue
                                    ? x.TotalGrossPrice * x.EuroRate.Value
                                    : x.TotalGrossPrice);
                            return new KeyValuePair<string, decimal>(location, value);
                        }).ToDictionary(k => k.Key, v => v.Value)
                    };

                    return new KeyValuePair<DateTime, LocationDeliveriesDto>(monthKey, locationDeliveries);
                }).ToDictionary(k => k.Key, v => v.Value);

            var sales = _unitOfWork.Get<Sale>()
                .GetAll(s => s.Date.HasValue && (!from.HasValue ||
                              s.Date.Value.Year >= from.Value.Year && s.Date.Value.Month >= from.Value.Month) &&
                             (!to.HasValue ||
                              from.HasValue && to.Value < from.Value ||
                              s.Date.Value.Year <= to.Value.Year && s.Date.Value.Month <= to.Value.Month),
                    s => s.Location)
                .OrderByDescending(s => s.Date)
                .ToList()
                .Where(s => s.Date.HasValue)
                .GroupBy(s => new { s.Date.Value.Year, s.Date.Value.Month })
                .Select(s =>
                {
                    var monthKey = new DateTime(s.Key.Year, s.Key.Month, 1);
                    var locationSales = new LocationSalesDto
                    {
                        Total = s.AsEnumerable().Sum(x => !x.TotalPrice.HasValue 
                            ? 0M 
                            : x.DollarRate.HasValue
                                ? x.TotalPrice.Value * x.DollarRate.Value
                                : x.EuroRate.HasValue
                                    ? x.TotalPrice.Value * x.EuroRate.Value
                                    : x.TotalPrice.Value),
                        LocationSales = s.GroupBy(i => i.Location).Select(i =>
                        {
                            var location = i.Key.Name;
                            var value = i.AsEnumerable().Sum(x => !x.TotalPrice.HasValue
                                ? 0M
                                : x.DollarRate.HasValue
                                    ? x.TotalPrice.Value * x.DollarRate.Value
                                    : x.EuroRate.HasValue
                                        ? x.TotalPrice.Value * x.EuroRate.Value
                                        : x.TotalPrice.Value);
                            return new KeyValuePair<string, decimal>(location, value);
                        }).ToDictionary(k => k.Key, v => v.Value)
                    };

                    return new KeyValuePair<DateTime, LocationSalesDto>(monthKey, locationSales);
                }).ToDictionary(k => k.Key, v => v.Value);

            var report = new TurnoverReportDto
            {
                Deliveries = deliveries,
                Sales = sales
            };

            return new SuccessResponse<TurnoverReportDto>(report);
        }
    }
}