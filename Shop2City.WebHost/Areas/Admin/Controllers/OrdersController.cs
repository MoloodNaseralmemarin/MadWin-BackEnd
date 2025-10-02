using MadWin.Application.Services;
using MadWin.Core.DTOs.Orders;
using Microsoft.AspNetCore.Mvc;
using Shop2City.WebHost.ViewModels.Orders;
using System.Threading.Tasks;

namespace Shop2City.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(OrderFilterParameters filter,int pageId=1)
        {
            var result = await _orderService.GetAllOrdersAsync(filter,pageId);
            return View(result);
        }


        public async Task<IActionResult> ShowOrderDetails(int id)
        {
            var getAllOrderDetails = await _orderService.GetCurtainComponentDetailsByOrderIdAsync(id);
            return PartialView("_OrderDetailsPartial", getAllOrderDetails);

        }

    }
}
