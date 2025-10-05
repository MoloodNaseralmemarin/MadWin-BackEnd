using MadWin.Application.Services;
using MadWin.Core.DTOs.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Shop2City.WebHost.ViewComponents
    {
        public class OrderSummaryComponent : ViewComponent
        {
            private readonly IOrderService _orderService;
            private readonly IDeliveryMethodService _deliveryMethodService;

            public OrderSummaryComponent(IOrderService orderService, IDeliveryMethodService deliveryMethodService)
            {
                _orderService = orderService;
                _deliveryMethodService = deliveryMethodService;
            }

        public async Task<IViewComponentResult> InvokeAsync(int userId)
        {
            var orders = await _orderService.GetTodayOrdersAsync(userId);
            var deliveryMethods = await _deliveryMethodService.GetDeliveryMethodInfoAsync();

            var vm = new OrderSummaryViewModel
            {
                OrderSummaryForAdmin = orders,
                DeliveryMethods = deliveryMethods,
            };

            return View("OrderSummary", vm);
        }

    }
}
