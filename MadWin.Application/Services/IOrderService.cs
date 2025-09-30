using MadWin.Core.DTOs.Calculations;
using MadWin.Core.DTOs.Orders;
using MadWin.Core.Entities.CurtainComponents;
using MadWin.Core.Entities.Orders;


namespace MadWin.Application.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<CurtainComponentProductGroupLookup>> GetCalculationAsync(int categoryId, int subCategoryId);
        Task<int> CreateOrderInitialAsync(CreateDto dto, int userId,decimal basePrice);

        Task UpdatePriceAndCommissionAsync(int orderId, decimal basePrice, int commissionFee, int commissionId);

        Task UpdatePriceAndDeliveryAsync(int deliveryId,int orderId);

        Task UpdateIsFinalyOrderAsync(Order order);
        Task<Order> GetOrderByOrderIdAsync(int orderId);

        Task<int> CountOrders();

        Task<OrderSummaryForAdminDto> GetTodayOrdersAsync(int pageId = 1);

        Task<IEnumerable<CurtainComponentDetail>> GetCurtainComponentDetailsByOrderIdAsync(int orderId);
        Task SoftDeleteFromOrderAsync(IEnumerable<int> orderIds);

        Task SoftDeleteAsync(IEnumerable<int> orderIds);

    }
}
