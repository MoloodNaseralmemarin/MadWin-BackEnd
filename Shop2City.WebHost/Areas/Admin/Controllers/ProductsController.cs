using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop2City.Core.Services.Products;

namespace Shop2City.WebHost.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> Index(int pageId = 1, string filterTitleProduct = "")
        {
            var products = await _productService.GetAllProducts(pageId,filterTitleProduct);
            return View(products);
        }
    }
}
