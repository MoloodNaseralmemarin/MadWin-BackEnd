
using MadWin.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Shop2City.WebHost.ViewModels.Orders;
using System.Threading.Tasks;

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
            var orders = await _orderService.GetTodayOrdersAsync(pageNumber, pageSize);
            var deliveryMethods = await _deliveryMethodService.GetDeliveryMethodInfoAsync();

            var viewModel = new OrderSummaryPageViewModel
            {
                Orders = orders,
                DeliveryMethods = deliveryMethods,
                OrderSummary = orders.Items.FirstOrDefault() 
            };

            return await Task.FromResult((IViewComponentResult)View("OrderSummary", viewModel));
        }

    }
}
