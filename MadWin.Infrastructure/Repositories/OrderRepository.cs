﻿using MadWin.Application.Services;
using MadWin.Core.DTOs.Orders;
using MadWin.Core.DTOs.Users;
using MadWin.Core.Entities.Common;
using MadWin.Core.Entities.CurtainComponents;
using MadWin.Core.Entities.Orders;
using MadWin.Core.Entities.Products;
using MadWin.Core.Entities.Users;
using MadWin.Core.Helpers;
using MadWin.Core.Interfaces;
using MadWin.Core.Lookups.Orders;
using MadWin.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MadWin.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly MadWinDBContext _context;
        private readonly ILogger<OrderRepository> _logger;
        private readonly INumberRoundingService _numberRoundingService;
        private readonly IDiscountRepository _discountRepository;
        private readonly IDeliveryMethodRepository _deliveryMethodRepository;
        private readonly IOrderWidthPartRepository _orderWidthPartRepository;
        private readonly ICurtainComponentDetailRepository _curtainComponentDetailRepository;
        public OrderRepository(MadWinDBContext context, ILogger<OrderRepository> logger, INumberRoundingService numberRoundingService, IDiscountRepository discountRepository, IDeliveryMethodRepository deliveryMethodRepository, IOrderWidthPartRepository orderWidthPartRepository, ICurtainComponentDetailRepository curtainComponentDetailRepository) : base(context)
        {
            _context = context;
            _logger = logger;
            _numberRoundingService = numberRoundingService;
            _discountRepository = discountRepository;
            _deliveryMethodRepository = deliveryMethodRepository;
            _orderWidthPartRepository = orderWidthPartRepository;
            _curtainComponentDetailRepository = curtainComponentDetailRepository;
        }

        public async Task<int> CreateOrderInitialAsync(Order order)
        {
            await _context.Set<Order>().AddAsync(order);
            await _context.SaveChangesAsync();
            return order.Id;
        }
        public async Task UpdatePriceAndCommissionAsync(int orderId, decimal basePrice, int commissionFee, int commissionId)
        {
            var resultBasePrice = _numberRoundingService.RoundLastThreeDigitsToZero(basePrice);

            const int FeePercentage = 100;

            decimal feeAmount = resultBasePrice * commissionFee / FeePercentage;

            var resultfeeAmount = _numberRoundingService.RoundLastThreeDigitsToZero(feeAmount);


            var order = await GetByIdAsync(orderId);
            if (order == null)
                _logger.LogWarning("Update failed: Order with ID {OrderId} not found.", orderId);

            order.BasePrice = resultBasePrice;
            order.PriceWithFee = resultBasePrice + resultfeeAmount;
            order.CommissionRateId = commissionId;
            order.CommissionPercent = commissionFee;
            order.CommissionAmount = resultfeeAmount;

            _context.Set<Order>().Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePriceAndDeliveryAsync(int deliveryId, int orderId)
        {
            var deliveryMethod = await _deliveryMethodRepository.GetDeliveryMethodInfoAsync(deliveryId);

            var order = await GetByIdAsync(orderId);
            if (order == null)
                _logger.LogWarning("Update failed: Order with ID {OrderId} not found.", orderId);

            order.DeliveryMethodId = deliveryMethod.DeliveryId;
            order.DeliveryMethodAmount = deliveryMethod.Cost;
            _context.Set<Order>().Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePriceAndDiscountAsync(int orderId, int discountId)
        {
            var discount = await _discountRepository.GetDiscountInfoByDiscountIdAsync(discountId);

            var order = await GetByIdAsync(orderId);
            if (order == null)
                _logger.LogWarning("Update failed: Order with ID {OrderId} not found.", orderId);

            order.DiscountId = discount.DiscountId;
            order.DisPercent = discount.DiscountPercent;

            #region به دست آوردن قیمت تخفیف
            var price = (order.PriceWithFee * discount.DiscountPercent) / 100;
            #endregion
            order.DisTotal = price;
            _context.Set<Order>().Update(order);
            await _context.SaveChangesAsync();
        }


        public async Task<Order> GetOrderByOrderIdAsync(int orderId)
        {
            return await _context.Set<Order>()
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<OrderInfoLookup> GetOrderInfoByOrderIdAsync(int orderId)
        {
            var order = await _context.Set<Order>()
                .Where(o => o.Id == orderId)
                .Select(o => new OrderInfoLookup
                {
                    OrderId = o.Id,
                    Price = o.PriceWithFee
                }).FirstOrDefaultAsync();

            if (order == null)
                return null;
            return order;
        }

        public async Task<int> CountOrders()
        {
            return await _context.Orders
                .Where(o => o.IsFinaly)
                .CountAsync();
        }

        public async Task SoftDeleteFromOrderAsync(IEnumerable<int> orderIds)
        {
            if (orderIds == null || !orderIds.Any())
                return;

            // سفارش‌ها
            var orders = await GetQuery()
                .Where(o => orderIds.Contains(o.Id) && !o.IsDelete)
                .ToListAsync();

            foreach (var o in orders)
                MarkAsDeleted(o);

            UpdateRange(orders);

            // عرض‌ها
            var widthParts = await _orderWidthPartRepository.GetQuery()
                .Where(w => orderIds.Contains(w.OrderId) && !w.IsDelete)
                .ToListAsync();

            foreach (var w in widthParts)
                MarkAsDeleted(w);

            _orderWidthPartRepository.UpdateRange(widthParts);

            // جزئیات پرده
            var curtainDetails = await _curtainComponentDetailRepository.GetQuery()
                .Where(c => orderIds.Contains(c.OrderId) && !c.IsDelete)
                .ToListAsync();

            foreach (var c in curtainDetails)
                MarkAsDeleted(c);

            _curtainComponentDetailRepository.UpdateRange(curtainDetails);

            // ذخیره
            await SaveChangesAsync();
        }

        private void MarkAsDeleted(BaseEntity entity)
        {
            entity.IsDelete = true;
            entity.LastUpdateDate = DateTime.Now;
            entity.Description = "توسط کاربر حذف شده است.";
        }


        public async Task<IEnumerable<CurtainComponentDetail>> GetByOrderIdAsync(int orderId)
        {
            return await _context.CurtainComponentDetails
                .Include(cd => cd.CurtainComponent)
                .Where(od => od.OrderId == orderId)

                .ToListAsync();

        }
        public async Task<OrderSummaryForAdminDto> GetTodayOrdersAsync(int userId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);


            // کوئری اصلی (فقط سفارش‌های همین کاربر)
            IQueryable<Order> query = GetQuery()
                .IgnoreQueryFilters()
                .Include(o => o.User)
                .Include(o => o.OrderCategory)
                .Include(o => o.OrderSubCategory)
                .Where(o => !o.IsDelete
                            && o.CreateDate >= today
                            && o.CreateDate < tomorrow
                            && o.UserId == userId); // 🔹 فیلتر بر اساس کاربر

            // ساخت DTO
            var list = new OrderSummaryForAdminDto
            {
                OrderSummary = await query
                    .OrderByDescending(o => o.Id)
                    .Select(o => new OrderSummaryForAdminItemDto
                    {
                        OrderId = o.Id,
                        CreateDate = o.CreateDate,
                        FullName = (o.User != null
                            ? (o.User.FirstName ?? "") + " " + (o.User.LastName ?? "")
                            : "نامشخص"),
                        CategoryGroup =
                            (o.OrderCategory != null ? o.OrderCategory.Title : "") +
                            (o.OrderSubCategory != null && !string.IsNullOrEmpty(o.OrderSubCategory.Title)
                                ? " / " + o.OrderSubCategory.Title
                                : ""),

                        Size = $"ارتفاع: {o.Height} - عرض: {o.Width}",
                        SizeSMS = $"w: {o.Width} * h: {o.Height}",
                        Count = o.Count,
                        PriceWithFee = o.PriceWithFee,
                        IsEqualParts = o.IsEqualParts,
                        PartCount = o.PartCount,
                        IsFinaly = o.IsFinaly,
                        WidthParts = new List<OrderWidthPartDto>() // همیشه لیست خالی
                    }).ToListAsync()
            };

            // جمع کل سفارش‌ها
            list.TotalPrice = list.OrderSummary?.Sum(o => (o.PriceWithFee) * o.Count) ?? 0;

            if (list.OrderSummary == null)
                throw new Exception("OrderSummary is null!");

            var orderIds = list.OrderSummary.Select(o => o.OrderId).ToList();

            if (orderIds.Any())
            {
                // گرفتن عرض‌های تکه‌ها
                var widthParts = await _context.Set<OrderWidthPart>()
                    .Where(w => orderIds.Contains(w.OrderId))
                    .Select(w => new
                    {
                        w.OrderId,
                        w.WidthValue
                    }).ToListAsync();

                // اضافه کردن تکه‌ها به هر سفارش
                foreach (var order in list.OrderSummary)
                {
                    var parts = widthParts
                        .Where(wp => wp.OrderId == order.OrderId)
                        .Select(wp => new OrderWidthPartDto
                        {
                            WidthValue = wp.WidthValue
                        }).ToList();

                    order.WidthParts = parts ?? new List<OrderWidthPartDto>();
                }
            }

            return list;
        }

        public async Task<OrderForAdminViewModel> GetAllOrdersAsync(OrderFilterParameters filter, int pageId = 1)
        {
            IQueryable<Order> query = GetQuery()
                .Include(o => o.User)
                .Include(o => o.OrderCategory)
                .Include(o => o.OrderSubCategory)
                .Where(o => !o.IsDelete);

            // فیلترها
            if (!string.IsNullOrWhiteSpace(filter.FullName))
            {
                query = query.Where(o =>
                    string.Concat(o.User.FirstName, " ", o.User.LastName).Contains(filter.FullName));
            }

            if (filter.OrderId.HasValue)
            {
                query = query.Where(o => o.Id == filter.OrderId.Value);
            }

            if (filter.FromDate.HasValue)
            {
                query = query.Where(o => o.CreateDate >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(o => o.CreateDate <= filter.ToDate.Value);
            }

            if (filter.FromPrice.HasValue)
            {
                query = query.Where(o => o.PriceWithFee >= filter.FromPrice.Value);
            }

            if (filter.ToPrice.HasValue)
            {
                query = query.Where(o => o.PriceWithFee <= filter.ToPrice.Value);
            }

            int take = 12;
            int skip = (pageId - 1) * take;

            var list = new OrderForAdminViewModel
            {
                CurrentPage = pageId,
                CountPage = (int)Math.Ceiling(query.Count() / (double)take),
                OrderSummary = await query
                    .OrderByDescending(u => u.Id)
                    .Skip(skip)
                    .Take(take)
                    .Select(o => new OrderSummaryForAdminItemDto
                    {
                        OrderId = o.Id,
                        CreateDate = o.CreateDate,
                        FullName = (o.User != null
                            ? (o.User.FirstName ?? "") + " " + (o.User.LastName ?? "")
                            : "نامشخص"),
                        CategoryGroup =
                            (o.OrderCategory != null ? o.OrderCategory.Title : "") +
                            (o.OrderSubCategory != null && !string.IsNullOrEmpty(o.OrderSubCategory.Title)
                                ? " / " + o.OrderSubCategory.Title
                                : ""),
                        CellPhone=o.User.CellPhone,

                        Address = (o.User != null
                            ? o.User.Address ?? ""
                            : "نامشخص"),
                        Size = $"ارتفاع: {o.Height} - عرض: {o.Width}",
                        SizeSMS = $"w: {o.Width} * h: {o.Height}",
                        Count = o.Count,
                        PriceWithFee = o.PriceWithFee,
                        IsEqualParts = o.IsEqualParts,
                        PartCount = o.PartCount,
                        IsFinaly = o.IsFinaly,
                        BasePrice=o.BasePrice,

                        DisPercent=o.DisPercent,
                        DisTotal=o.DisTotal,
                        TotalPrice=o.TotalCost,
                        WidthParts = new List<OrderWidthPartDto>() // همیشه لیست خالی
                    }).ToListAsync()
            };


            if (list.OrderSummary == null)
                throw new Exception("OrderSummary is null!");

            var orderIds = list.OrderSummary.Select(o => o.OrderId).ToList();

            if (orderIds.Any())
            {
                // گرفتن عرض‌های تکه‌ها
                var widthParts = await _context.Set<OrderWidthPart>()
                    .Where(w => orderIds.Contains(w.OrderId))
                    .Select(w => new
                    {
                        w.OrderId,
                        w.WidthValue
                    }).ToListAsync();

                // اضافه کردن تکه‌ها به هر سفارش
                foreach (var order in list.OrderSummary)
                {
                    var parts = widthParts
                        .Where(wp => wp.OrderId == order.OrderId)
                        .Select(wp => new OrderWidthPartDto
                        {
                            WidthValue = wp.WidthValue
                        }).ToList();

                    order.WidthParts = parts ?? new List<OrderWidthPartDto>();
                }
            }
            return list;
        }
        public async Task<OrderForAdminViewModel> GetAllOrdersByUserIdAsync(int userId, OrderFilterParameters filter, int pageId = 1)
        {
            // کوئری اصلی (فقط سفارش‌های همین کاربر)
            IQueryable<Order> query = GetQuery()
                .Include(o => o.User)
                .Include(o => o.OrderCategory)
                .Include(o => o.OrderSubCategory)
                .Where(o => !o.IsDelete && o.UserId == userId);

            // فیلترها
            if (!string.IsNullOrWhiteSpace(filter.FullName))
            {
                query = query.Where(o =>
                    string.Concat(o.User.FirstName, " ", o.User.LastName).Contains(filter.FullName));
            }

            if (filter.OrderId.HasValue)
            {
                query = query.Where(o => o.Id == filter.OrderId.Value);
            }

            if (filter.FromDate.HasValue)
            {
                query = query.Where(o => o.CreateDate >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(o => o.CreateDate <= filter.ToDate.Value);
            }

            if (filter.FromPrice.HasValue)
            {
                query = query.Where(o => o.PriceWithFee >= filter.FromPrice.Value);
            }

            if (filter.ToPrice.HasValue)
            {
                query = query.Where(o => o.PriceWithFee <= filter.ToPrice.Value);
            }

            int take = 12;
            int skip = (pageId - 1) * take;

            var list = new OrderForAdminViewModel
            {
                CurrentPage = pageId,
                CountPage = (int)Math.Ceiling(query.Count() / (double)take),
                OrderSummary = await query
                    .OrderByDescending(u => u.Id)
                    .Skip(skip)
                    .Take(take)
                    .Select(o => new OrderSummaryForAdminItemDto
                    {
                        OrderId = o.Id,
                        CreateDate = o.CreateDate,
                        FullName = (o.User != null
                            ? (o.User.FirstName ?? "") + " " + (o.User.LastName ?? "")
                            : "نامشخص"),
                        CategoryGroup =
                            (o.OrderCategory != null ? o.OrderCategory.Title : "") +
                            (o.OrderSubCategory != null && !string.IsNullOrEmpty(o.OrderSubCategory.Title)
                                ? " / " + o.OrderSubCategory.Title
                                : ""),

                        Size = $"ارتفاع: {o.Height} - عرض: {o.Width}",
                        SizeSMS = $"w: {o.Width} * h: {o.Height}",
                        Count = o.Count,
                        PriceWithFee = o.PriceWithFee,
                        IsEqualParts = o.IsEqualParts,
                        PartCount = o.PartCount,
                        IsFinaly = o.IsFinaly,
                        BasePrice = o.BasePrice,
                        TotalPrice = o.TotalCost,
                        WidthParts = new List<OrderWidthPartDto>() // همیشه لیست خالی
                    }).ToListAsync()
            };


            if (list.OrderSummary == null)
                throw new Exception("OrderSummary is null!");

            var orderIds = list.OrderSummary.Select(o => o.OrderId).ToList();

            if (orderIds.Any())
            {
                // گرفتن عرض‌های تکه‌ها
                var widthParts = await _context.Set<OrderWidthPart>()
                    .Where(w => orderIds.Contains(w.OrderId))
                    .Select(w => new
                    {
                        w.OrderId,
                        w.WidthValue
                    }).ToListAsync();

                // اضافه کردن تکه‌ها به هر سفارش
                foreach (var order in list.OrderSummary)
                {
                    var parts = widthParts
                        .Where(wp => wp.OrderId == order.OrderId)
                        .Select(wp => new OrderWidthPartDto
                        {
                            WidthValue = wp.WidthValue
                        }).ToList();

                    order.WidthParts = parts ?? new List<OrderWidthPartDto>();
                }
            }
            return list;
        }
    }
}

      