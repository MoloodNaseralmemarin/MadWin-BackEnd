using MadWin.Application.DTOs.Reports;
using MadWin.Core.Entities.Orders;
using MadWin.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadWin.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IFactorRepository _factorRepository;
        public ReportService(IOrderRepository orderRepository, IFactorRepository factorRepository)
        {
            _orderRepository = orderRepository;
            _factorRepository = factorRepository;
        }

        public async Task<UserFactorPrintDto?> GetUserFactorPrintByFactorId(int factorId)
        {
            var result = await _factorRepository.GetQuery()
                            .Include(o => o.User)
                            .Include(o => o.FactorDetails)
                            .ThenInclude(f=>f.Product)
                            .FirstOrDefaultAsync(o => o.Id == factorId);

            if (result is null)
                return null;

            return new()
            {

                Price = result.TotalAmount,
                Description = result.Description,
                ProductName =$"result.FactorDetails.Select(d => d.Product.Title)".Trim(),
                FullName = $"{result.User?.FirstName} {result.User?.LastName}".Trim(),
                Address = result.User?.Address,
                CreatedAt = result.CreatedAt,
            };
        }
        
        public async Task<UserOrderPrintDto?> GetUserOrderPrintByOrderId(int orderId)
        {
            var result = await _orderRepository.GetQuery()
                .Include(o => o.User)
                .Include(o => o.OrderCategory)
                .Include(o => o.OrderSubCategory)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (result is null)
                return null;

            return new()
            {

                Price = result.TotalAmount,
                Description = result.Description,
                OrderName = $"{result.OrderCategory?.Title} {result.OrderSubCategory?.Title}".Trim(),
                FullName = $"{result.User?.FirstName} {result.User?.LastName}".Trim(),
                Address = result.User?.Address,
                CreatedAt=result.CreatedAt,
            };
        }
    }
}
