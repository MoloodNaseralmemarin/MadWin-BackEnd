using MadWin.Application.Services;
using MadWin.Core.DTOs.Orders;
using MadWin.Core.Lookups.DeliveryMethods;
using Microsoft.AspNetCore.Mvc;
using Shop2City.WebHost.ViewModels.DeliveryMethods;
using Shop2City.WebHost.ViewModels.Orders;

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

            public async Task<IViewComponentResult> InvokeAsync(int pageNumber = 1, int pageSize = 10)
            {
                var orders = await _orderService.GetTodayOrdersAsync(pageNumber, pageSize)
                             ?? new PagedResult<OrderSummaryDto> { Items = new List<OrderSummaryDto>(), TotalCount = 0 };

                var deliveryMethods = await _deliveryMethodService.GetDeliveryMethodInfoAsync()
                                      ?? new List<DeliveryMethodInfoLookup>();

                var vm = new OrderSummaryPageViewModel
                {
                    OrderSummary = orders,
                    DeliveryMethods = deliveryMethods
                };

            return await Task.FromResult((IViewComponentResult)View("OrderSummary", vm));
        }
        }
    }
