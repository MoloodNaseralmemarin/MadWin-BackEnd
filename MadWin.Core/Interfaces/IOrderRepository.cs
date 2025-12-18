using MadWin.Core.DTOs.Factors;
using MadWin.Core.DTOs.Orders;
using MadWin.Core.Entities.CurtainComponents;
using MadWin.Core.Entities.Orders;
using MadWin.Core.Lookups.Orders;

namespace MadWin.Core.Interfaces
{
    public interface IOrderRepository:IGenericRepository<Order>
    {
        Task<int> CreateOrderInitialAsync(Order order);

        Task UpdatePriceAndCommissionAsync(int orderId, decimal basePrice, int commissionFee, int commissionId);

        Task UpdatePriceAndDeliveryAsync(int deliveryId,int orderId);

        Task<Order> GetOrderByOrderIdAsync(int orderId);

        Task<OrderInfoLookup> GetOrderInfoByOrderIdAsync(int orderId);
        Task<int> CountOrders();

        Task<decimal> GetTotalOrdersPriceAsync();

        Task<decimal> GetTodayTotalOrdersPriceAsync();

        Task SoftDeleteFromOrderAsync(IEnumerable<int> orderIds);

        Task<IEnumerable<CurtainComponentDetail>> GetByOrderIdAsync(int orderId);

        Task<OrderSummaryForAdminDto> GetTodayOrdersAsync(int userId);

        Task<OrderForAdminViewModel> GetAllOrdersAsync(OrderFilterParameter filter, int pageId = 1);

        Task<OrderForAdminViewModel> GetAllOrdersByUserIdAsync(int userId, OrderFilterParameter filter, int pageId = 1);

        Task<OrderSummaryForAdminItemDto> GetOrdersByOrderIdAsync(int orderId);

        Task<OrderDetailForPrint> GetOrderDetailForPrint(int orderId);



        Task<IEnumerable<Order>> GetAllByUserAndDateAsync(int userId, DateTime date);
    
    }
}
