using MadWin.Application.Services;
using MadWin.Core.DTOs.Factors;
using Microsoft.AspNetCore.Mvc;

namespace Shop2City.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICurtainComponentDetailService _curtainComponentDetailService;

        public OrdersController(IOrderService orderService, ICurtainComponentDetailService curtainComponentDetailService)
        {
            _orderService = orderService;
            _curtainComponentDetailService = curtainComponentDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(OrderFilterParameter filter,int pageId=1)
        {
            var result = await _orderService.GetAllOrdersAsync(filter,pageId);
            return View(result);
        }

        public async Task<IActionResult> GetDailyOrderSummary(OrderFilterParameter filter, int pageId = 1)
        {
            var result = await _orderService.GetDailyOrderSummaryAsync(filter);
            return View(result);
        }
        public async Task<IActionResult> ShowOrderDetails(int id)
        {
            var getAllOrderDetails = await _curtainComponentDetailService.GetCurtainComponentDetailByOrderIdAsync(id);
            return PartialView("_OrderDetailsPartial", getAllOrderDetails);

        }

    }
}
