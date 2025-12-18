using MadWin.Application.DTOs.Reports;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadWin.Application.Services
{
    public interface IReportService
    {
        Task<UserOrderPrintDto?> GetUserOrderPrintByOrderId(int orderId);
        Task<UserFactorPrintDto?> GetUserFactorPrintByFactorId(int factorId);
    }
}
