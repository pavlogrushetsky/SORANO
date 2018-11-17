using System;
using System.Linq;
using SORANO.BLL.Dtos;
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
            var items = _unitOfWork.Get<Delivery>()
                .GetAll(d => (!from.HasValue ||
                              d.DeliveryDate.Year >= from.Value.Year && d.DeliveryDate.Month >= from.Value.Month) &&
                             (!to.HasValue ||
                              from.HasValue && to.Value < from.Value ||
                              d.DeliveryDate.Year <= to.Value.Year && d.DeliveryDate.Month <= to.Value.Month))
                .OrderByDescending(d => d.DeliveryDate)
                .GroupBy(d => new {d.DeliveryDate.Year, d.DeliveryDate.Month})
                .SelectMany(d => d.GroupBy(i => i.DeliveryLocation).Select(i => new MonthReportItemDto
                {
                    Month = d.Key.Month,
                    Year = d.Key.Year,
                    LocationValues = i.AsEnumerable().Select(x => new LocationValueReportItemDto
                    {
                        LocationName = x.DeliveryLocation.Name,
                        Value = x.TotalGrossPrice
                    }).ToList(),
                    Total = i.AsEnumerable().Sum(x => x.TotalGrossPrice)
                })).ToList();

            var report = new TurnoverReportDto
            {
                Items = items
            };

            return new SuccessResponse<TurnoverReportDto>(report);
        }
    }
}