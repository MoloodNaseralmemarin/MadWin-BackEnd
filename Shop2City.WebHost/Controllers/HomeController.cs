using MadWin.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shop2City.Core.Services.Products;

namespace Shop2City.WebHost.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IDeliveryMethodService _deliveryMethodService;
        public HomeController(IProductService productService, IDeliveryMethodService deliveryMethodService)
        {
            _productService = productService;
            _deliveryMethodService = deliveryMethodService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetDeliveryPriceAsync(int deliveryId)
        {
            var pricePost = await _deliveryMethodService.GetDeliveryMethodCostByIdAsync(deliveryId);

            return Ok(new
            {
                success = true,
                price = pricePost
            });
        }

    }
}
