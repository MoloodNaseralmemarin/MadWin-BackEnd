using MadWin.Application.Services;
using MadWin.Core.DTOs.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop2City.WebHost.ViewModels.Orders;
using System.Threading.Tasks;

namespace Shop2City.Web.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(OrderFilterParameters filter, int pageNumber = 1, int pageSize = 12)
        {
            var result = await _orderService.GetOrderSummaryAsync(filter);

            var vm = new OrderSummaryForAdminViewModel
            {
                Filter = filter,
                OrderSummary = result,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize)
            };
            return View(vm);
        }


        public async Task<IActionResult> ShowOrderDetails(int id)
        {
            var getAllOrderDetails = await _orderService.GetCurtainComponentDetailsByOrderIdAsync(id);
            return PartialView("_OrderDetailsPartial", getAllOrderDetails);

        }

    }
}
