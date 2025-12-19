using MadWin.Core.DTOs.Calculations;
using MadWin.Core.DTOs.Factors;
using MadWin.Core.DTOs.Orders;
using MadWin.Core.Entities.Common;
using MadWin.Core.Entities.Orders;
using MadWin.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop2City.Core.Convertors;

namespace MadWin.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderWidthPartRepository _orderWidthPartRepository;
        private readonly ICurtainComponentProductGroupRepository _curtainComponentProductGroupRepository;
        public OrderService(ILogger<OrderService> logger, IOrderRepository orderRepository, IOrderWidthPartRepository orderWidthPartRepository, ICurtainComponentProductGroupRepository curtainComponentProductGroupRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _orderWidthPartRepository= orderWidthPartRepository;
            _curtainComponentProductGroupRepository= curtainComponentProductGroupRepository;

        }
        public async Task<int> CreateOrderInitialAsync(CreateDto dto, int userId, decimal basePrice)
        {
            var order = new Order
            {
                UserId = userId,
                CategoryId = dto.CategoryId,
                SubCategoryId = dto.SubCategoryId,
                Height = dto.Height,
                Width = dto.Width,
                Count = dto.Count,
                BasePrice = basePrice,
                PartCount = dto.PartCount,
                IsEqualParts = dto.IsEqualParts,
                IsCurtainAdhesive=dto.IsCurtainAdhesive
            };

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync(); // اینجا Id ساخته می‌شود

            if (dto.WidthParts != null && dto.WidthParts.Any())
            {
                foreach (var width in dto.WidthParts)
                {
                    var widthPart = new OrderWidthPart
                    {
                        OrderId = order.Id,
                        WidthValue = width,
                        Description = "درج نشده است"

                    };
                    await _orderWidthPartRepository.AddAsync(widthPart);
                }
                await _orderWidthPartRepository.SaveChangesAsync();
            }

            return order.Id;
        }

        public async Task UpdatePriceAndCommissionAsync(int orderId, decimal basePrice, int commissionFee, int commissionId)
        {
            try
            {
                await _orderRepository.UpdatePriceAndCommissionAsync(orderId, basePrice, commissionFee, commissionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId} in OrderService", orderId);
                throw;
            }
        }
        public async Task<IEnumerable<CurtainComponentProductGroupLookup>> GetCalculationAsync(int categoryId, int subCategoryId)
        {
            var result = await _curtainComponentProductGroupRepository.CalculationByCategory(categoryId, subCategoryId);
            return result;
        }
 
        public async Task<Order> GetOrderByOrderIdAsync(int orderId)
        {
            return await _orderRepository.GetByIdAsync(orderId);
        }


        public async Task UpdateIsFinalyOrderAsync(int userId)
        {
            var today = DateTime.Today;

            var orders = await _orderRepository.GetAllByUserAndDateAsync(userId, today);

            if (orders == null || !orders.Any()) return;

            foreach (var order in orders)
            {
                order.IsFinaly = true;
                order.TotalAmount = order.PriceWithFee + order.DeliveryMethodAmount - order.DisTotal;
                order.TotalCost = order.TotalAmount;
            }

            await _orderRepository.SaveChangesAsync();
        }
        public async Task UpdatePriceAndDeliveryAsync(int deliveryId, int orderId)
        {
            try
            {
                await _orderRepository.UpdatePriceAndDeliveryAsync(deliveryId,orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId} in OrderService", orderId);
                throw;
            }
        }

        public async Task<int> CountOrders()
        {
            return await _orderRepository.CountOrders();
        }

        public async Task<decimal> GetTotalOrdersPriceAsync()
        {
            return await _orderRepository.GetTotalOrdersPriceAsync();
        }

        public async Task<decimal> GetTodayTotalOrdersPriceAsync()
        {
            return await _orderRepository.GetTodayTotalOrdersPriceAsync();
        }
        public async Task<OrderSummaryForAdminDto> GetTodayOrdersAsync(int userId)
        {
            return await _orderRepository.GetTodayOrdersAsync(userId);
        }



        public class SoftDeleteResult
        {
            public List<int> DeletedOrderIds { get; set; } = new();
            public List<int> DeletedWidthPartIds { get; set; } = new();
            public List<int> DeletedCurtainDetailIds { get; set; } = new();
        }

        public async Task SoftDeleteFromOrderAsync(int[] orderId)
        {
            if (orderId == null || !orderId.Any()) return;
            var allOrders = _orderRepository.GetQuery();
            var ordersToDelete = allOrders.Where(o => orderId.Contains(o.Id) && !o.IsDelete).ToList();
            if (!ordersToDelete.Any()) return;

            var factorIds = ordersToDelete.Select(o => o.Id).Distinct();
            foreach (var item in ordersToDelete)
            {
                MarkAsDeleted(item);
                _orderRepository.Update(item);
            }
            await _orderRepository.SaveChangesAsync();
            var allOrderWidthParts = await _orderWidthPartRepository.GetAllAsync();
            var orderWidthPartsToDelete = allOrderWidthParts.Where(o => orderId.Contains(o.Id) && !o.IsDelete).ToList();
            foreach (var item in orderWidthPartsToDelete)
            {
                MarkAsDeleted(item);
                _orderWidthPartRepository.Update(item);
            }
            await _orderWidthPartRepository.SaveChangesAsync();
            return;
        }
        private void MarkAsDeleted(BaseEntity entity)
        {
            entity.IsDelete = true;
            entity.UpdatedAt = DateTime.Now;
            entity.Description = "توسط کاربر حذف شده است.";
        }

        public async Task<decimal> GetSumPriceWithFeeByOrder(int[] orderIds, int userId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var allOrders = await _orderRepository.GetAllAsync();
            var orders = allOrders
                .Where(o => orderIds.Contains(o.Id) &&
                            o.UserId == userId &&
                            o.CreatedAt >= today && o.CreatedAt < tomorrow);

            return orders.Sum(o => o.PriceWithFee);
        }

        public async Task<OrderForAdminViewModel> GetAllOrdersAsync(OrderFilterParameter filter, int pageId = 1)
        {
            var order = await _orderRepository.GetAllOrdersAsync(filter,pageId);

            if (order == null)
            {
                _logger.LogWarning("هیچ سفارشی پیدا نشد.");
                return null;
            }

            _logger.LogInformation("سفارش  با موفقیت بارگذاری شد.");
            return order; 
        }

        public async Task<OrderForAdminViewModel> GetAllOrdersByUserIdAsync(int userId, OrderFilterParameter filter, int pageId = 1)
        {
            var order = await _orderRepository.GetAllOrdersByUserIdAsync(userId,filter, pageId);

            if (order == null)
            {
                _logger.LogWarning("هیچ سفارشی پیدا نشد.");
                return null;
            }

            _logger.LogInformation("سفارش  با موفقیت بارگذاری شد.");
            return order;
        }

        public async Task AddDescriptionForOrder(int orderId, string description)
        {
            var order=await _orderRepository.GetByIdAsync(orderId);
            order.Description= description;
            await _orderRepository.SaveChangesAsync();
        }
        #region OrderDetailForPrint
        public async Task<OrderDetailForPrint> GetOrderDetailForPrint(int orderId)
        {
            return await _orderRepository.GetOrderDetailForPrint(orderId);
        }
        #endregion

        public async Task<int> GetTodaySettledOrdersCount()
        {
            var today = DateTime.Today;

            return await _orderRepository.GetQuery()
                .Where(o => o.IsFinaly &&
                            o.CreatedAt.Date == today &&
                            !o.IsDelete)
                .CountAsync();
        }


        public async Task<int> GetTodayUnSettledOrdersCount()
        {
            var today = DateTime.Today;

            return await _orderRepository.GetQuery()
                .Where(o => !o.IsFinaly &&
                            o.CreatedAt.Date == today &&
                            !o.IsDelete)
                .CountAsync();
        }


        public async Task<List<DailyOrderSummaryDto>> GetDailyOrderSummaryAsync(OrderFilterParameter filter)
        {
            DateTime? fromDate = string.IsNullOrWhiteSpace(filter.FromDate)
                ? null
                : DateConvertor.ConvertPersianToGregorian(filter.FromDate);

            DateTime? toDate = string.IsNullOrWhiteSpace(filter.ToDate)   // ← اصلاح شد، قبلاً اشتباه FromDate بود
                ? null
                : DateConvertor.ConvertPersianToGregorian(filter.ToDate); // شامل کل روز

            var query = _orderRepository
                .GetQuery()
                .AsNoTracking()
                .Include(o => o.User)
                .Where(o => !o.IsDelete);

            if (fromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(o => o.CreatedAt < toDate.Value);

            var result = await query
         .GroupBy(o => new
         {
             FullName = o.User != null
                 ? (o.User.FirstName ?? "") + " " + (o.User.LastName ?? "")
                 : "نامشخص",
             Date = o.CreatedAt.Date,
             CellPhone= o.User != null
                 ? (o.User.CellPhone ?? "")
                 : "نامشخص",
         })
         .Select(g => new DailyOrderSummaryDto
         {
             FullName = g.Key.FullName,
             Date = g.Key.Date,
            
             FinalOrderCount = g.Count(x => x.IsFinaly),
             FinalTotalPrice = g.Where(x => x.IsFinaly).Sum(x => (decimal?)x.TotalCost) ?? 0,

             OpenOrderCount = g.Count(x => !x.IsFinaly),
             OpenTotalPrice = g.Where(x => !x.IsFinaly).Sum(x => (decimal?)x.TotalCost) ?? 0,

             IsFinaly = g.Any(x => x.IsFinaly),

             Orders = g.Select(x => new OrderDetailDto
             {
                 OrderId = x.Id,
                 CreatedAt = x.CreatedAt,
                 FullName = g.Key.FullName,
                 CellPhone = x.User != null ? x.User.CellPhone : "",
                 Address = x.User != null ? x.User.Address ?? "" : "نامشخص",
                 DeliveryMethodName = x.DeliveryMethod.Name,
                 DeliveryMethodCost = x.DeliveryMethodAmount,
                 DisPercent = x.DisPercent,
                 DisTotal = x.DisTotal,
                 CategoryGroup =
                     (x.OrderCategory != null ? x.OrderCategory.Title : "") +
                     (x.OrderSubCategory != null && !string.IsNullOrEmpty(x.OrderSubCategory.Title)
                         ? " / " + x.OrderSubCategory.Title
                         : ""),
                 Size = $"ارتفاع: {x.Height} - عرض: {x.Width}",
                 Count = x.Count,
                 IsCurtainAdhesive = x.IsCurtainAdhesive,
                 IsEqualParts=x.IsEqualParts,
                 BasePrice = x.BasePrice,
                 TotalPrice = x.TotalCost,
                 IsFinaly = x.IsFinaly,
                 Description = x.Description,
                 WidthParts = x.WidthParts.Select(w => new OrderWidthPartDto
                 {
                     WidthValue = w.WidthValue
                 }).ToList()
             }).ToList()
         })
         .OrderByDescending(x => x.Date)
         .ThenBy(x => x.FullName)
         .ToListAsync();

            return result;
        }

    }

}
