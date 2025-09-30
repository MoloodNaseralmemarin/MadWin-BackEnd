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

            public async Task<IViewComponentResult> InvokeAsync(int pageId = 1)
            {
            var orders = await _orderService.GetTodayOrdersAsync(pageId);
            var deliveryMethods = await _deliveryMethodService.GetDeliveryMethodInfoAsync();

            var vm = new OrderSummaryViewModel
            {
                OrderSummaryForAdmin = orders,
                DeliveryMethods = deliveryMethods,

            };
            

            return await Task.FromResult((IViewComponentResult)View("OrderSummary", vm));
        }
        }
    }
