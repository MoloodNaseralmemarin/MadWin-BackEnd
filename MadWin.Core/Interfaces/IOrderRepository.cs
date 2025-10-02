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

        Task SoftDeleteFromOrderAsync(IEnumerable<int> orderIds);

        Task<IEnumerable<CurtainComponentDetail>> GetByOrderIdAsync(int orderId);

        Task<OrderSummaryForAdminDto> GetTodayOrdersAsync(int userId);

        Task<OrderForAdminViewModel> GetAllOrdersAsync(OrderFilterParameters filter, int pageId = 1);
    }
}
