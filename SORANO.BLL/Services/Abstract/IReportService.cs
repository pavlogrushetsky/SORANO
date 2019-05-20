using System;
using SORANO.BLL.Dtos.ReportDtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IReportService
    {
        ServiceResponse<TurnoverReportDto> GetTurnoverReport(DateTime? from, DateTime? to);

        ServiceResponse<InventoryReportDto> GetInventoryReport(int? locationId);
    }
}