using System;
using SORANO.BLL.Dtos;

namespace SORANO.BLL.Services.Abstract
{
    public interface IReportService
    {
        ServiceResponse<TurnoverReportDto> GetTurnoverReport(DateTime? from, DateTime? to);
    }
}