using MadWin.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shop2City.WebHost.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ProductGroupsController : Controller
    {
        private readonly IProductGroupService _productGroupService;
        public ProductGroupsController(IProductGroupService productGroupService)
        {
            _productGroupService = productGroupService;
        }
        public async Task<IActionResult> Index(int pageId = 1, string filterTitleProduct = "")
        {
            var products = await _productGroupService.GetAllProductGroupsAsync(pageId, filterTitleProduct);
            return View(products);
        }
    }
}