using MadWin.Application.Services;
using MadWin.Core.DTOs.Factors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Shop2City.WebHost.Areas.UserPanel.Controllers
{
        [Area("UserPanel")]
        [Authorize]
        public class OrdersController : Controller
        {
            private readonly IOrderService _orderService;

            public OrdersController(IOrderService orderService)
            {
                _orderService = orderService;
            }

            [HttpGet]
            public async Task<IActionResult> Index(OrderFilterParameter filter, int pageId = 1)
            {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            var result = await _orderService.GetAllOrdersByUserIdAsync(userId,filter, pageId);
                return View(result);
            }

        }
    }