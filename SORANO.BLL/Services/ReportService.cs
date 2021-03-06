﻿using System;
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

        public ServiceResponse<InventoryReportDto> GetInventoryReport(int? locationId)
        {
            var locationGoods = new Dictionary<string, IEnumerable<LocationGoodsDto>>();

            if (locationId.HasValue && locationId.Value > 0)
            {
                var location = _unitOfWork.Get<Location>().Get(locationId.Value);             
                locationGoods.Add(location.Name, GetLocationGoods(locationId.Value));
                
            }
            else
            {
                _unitOfWork
                    .Get<Location>()
                    .GetAll(l => !l.IsDeleted)
                    .ToList()
                    .ForEach(l => 
                    {
                        locationGoods.Add(l.Name, GetLocationGoods(l.ID));
                    });
            }
            
            return new SuccessResponse<InventoryReportDto>(new InventoryReportDto
            {
                LocationGoods = locationGoods
            });
        }

        private IEnumerable<LocationGoodsDto> GetLocationGoods(int locationId) => 
            _unitOfWork
                .Get<Storage>()
                .GetAll(s => s.LocationID == locationId && !s.ToDate.HasValue)
                .Where(g => !g.Goods.IsDeleted && !g.Goods.IsSold && g.Goods.SaleID == null)
                .GroupBy(g => new
                {
                    ArticleId = g.Goods.DeliveryItem.ArticleID,
                    ArticleName = g.Goods.DeliveryItem.Article.Name,
                    ArticleCode = g.Goods.DeliveryItem.Article.Code,
                    ArticleType = g.Goods.DeliveryItem.Article.Type.ParentType == null 
                        ? g.Goods.DeliveryItem.Article.Type.Name 
                        : g.Goods.DeliveryItem.Article.Type.ParentType.Name + " :: " + g.Goods.DeliveryItem.Article.Type.Name
                })
                .Select(g => new LocationGoodsDto
                {
                    ArticleId = g.Key.ArticleId,
                    ArticleName = g.Key.ArticleName,
                    ArticleCode = g.Key.ArticleCode,
                    ArticleType = g.Key.ArticleType,
                    Quantity = g.AsEnumerable().Count()
                })
                .OrderBy(l => l.ArticleName)
                .ToList();

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

            Dictionary<DateTime, LocationSalesDto> GetSales(bool isWriteOff, bool isProfit)
            {
                return _unitOfWork.Get<Sale>()
                    .GetAll(s => s.Date.HasValue && 
                            s.IsSubmitted && 
                            (isProfit || s.IsWriteOff == isWriteOff) && 
                            (!from.HasValue || s.Date.Value.Year >= from.Value.Year && s.Date.Value.Month >= from.Value.Month) && 
                            (!to.HasValue || from.HasValue && to.Value < from.Value || s.Date.Value.Year <= to.Value.Year && s.Date.Value.Month <= to.Value.Month), 
                        s => s.Location)
                    .Select(s => new
                    {
                        Sale = s,
                        s.Location,
                        DeliveryPrice = s.Goods.Sum(g => g.DeliveryItem.Delivery.DollarRate.HasValue
                            ? g.DeliveryItem.UnitPrice * g.DeliveryItem.Delivery.DollarRate.Value
                            : g.DeliveryItem.Delivery.EuroRate.HasValue
                                ? g.DeliveryItem.UnitPrice * g.DeliveryItem.Delivery.EuroRate.Value
                                : g.DeliveryItem.UnitPrice)
                    })
                    .OrderByDescending(s => s.Sale.Date)
                    .ToList()
                    .Where(s => s.Sale.Date.HasValue)
                    .GroupBy(s => new {s.Sale.Date.Value.Year, s.Sale.Date.Value.Month})
                    .Select(s =>
                    {
                        var monthKey = new DateTime(s.Key.Year, s.Key.Month, 1);
                        var locationSales = new LocationSalesDto
                        {
                            LocationSales = s.GroupBy(i => i.Location)
                                .Select(i =>
                                {
                                    var location = i.Key.Name;
                                    var cach = i.AsEnumerable().Where(x => !x.Sale.IsCachless).Sum(x => Sum(x.Sale, x.DeliveryPrice, isWriteOff, isProfit));
                                    var cachless = i.AsEnumerable().Where(x => x.Sale.IsCachless).Sum(x => Sum(x.Sale, x.DeliveryPrice, isWriteOff, isProfit));
                                    return new KeyValuePair<string, SaleDto>(location, new SaleDto { Cash = cach, Cashless = cachless });
                                })
                                .ToDictionary(k => k.Key, v => v.Value)
                        };

                        return new KeyValuePair<DateTime, LocationSalesDto>(monthKey, locationSales);
                    })
                    .ToDictionary(k => k.Key, v => v.Value);
            }

            decimal Sum(Sale sale, decimal deliveryPrice, bool isWriteOff, bool isProfit)
            {
                var saleHasPrice = sale.TotalPrice.HasValue;
                var salePrice = !saleHasPrice ? 0.0M : sale.TotalPrice.Value;
                var saleHasDollarRate = sale.DollarRate.HasValue;
                var dollarRate = saleHasDollarRate ? sale.DollarRate.Value : 1.0M;
                var saleHasEuroRate = sale.EuroRate.HasValue;
                var euroRate = saleHasEuroRate ? sale.EuroRate.Value : 1.0M;

                return isProfit
                    ? salePrice - deliveryPrice
                    : isWriteOff
                        ? deliveryPrice
                        : saleHasDollarRate
                            ? salePrice * dollarRate
                            : saleHasEuroRate
                                ? salePrice * euroRate
                                : salePrice;
            }

            var sales = GetSales(false, false);
            var writeoffs = GetSales(true, false);
            var profit = GetSales(false, true);

            var report = new TurnoverReportDto
            {
                Deliveries = deliveries,
                Sales = sales,
                Writeoffs = writeoffs,
                Profit = profit
            };

            return new SuccessResponse<TurnoverReportDto>(report);
        }
    }
}