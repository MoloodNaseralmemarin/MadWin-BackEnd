using MadWin.Core.DTOs.Calculations;
using MadWin.Core.DTOs.Factors;
using MadWin.Core.DTOs.Orders;
using MadWin.Core.Entities.Common;
using MadWin.Core.Entities.Orders;
using MadWin.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
                        WidthValue = width
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
        public async Task UpdateIsFinalyOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return;

            order.IsFinaly = true;
            order.TotalAmount = order.PriceWithFee + order.DeliveryMethodAmount - order.DisTotal;
            order.TotalCost = order.TotalAmount; //بعدااکش کن به درد نمیخوره

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
            entity.LastUpdateDate = DateTime.Now;
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
                            o.CreateDate >= today && o.CreateDate < tomorrow);

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
                            o.CreateDate.Date == today &&
                            !o.IsDelete)
                .CountAsync();
        }


        public async Task<int> GetTodayUnSettledOrdersCount()
        {
            var today = DateTime.Today;

            return await _orderRepository.GetQuery()
                .Where(o => !o.IsFinaly &&
                            o.CreateDate.Date == today &&
                            !o.IsDelete)
                .CountAsync();
        }
    }

}
