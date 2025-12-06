using MadWin.Application.DTOs.Products;
using MadWin.Core.Entities.CommissionRates;
using MadWin.Core.Entities.CurtainComponents;
using MadWin.Core.Entities.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop2City.Core.Services.Products;
using Shop2City.WebHost.ViewModels.CommissionRates;
using Shop2City.WebHost.ViewModels.CurtainComponents;

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
        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var vm = new EditProductDto
            {
                Id = product.Id,
                Price = product.Price,
                Title = product.Title,
                ShortDescription=product.ShortDescription,
            };

            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProductDto vm)
        {
            // اعتبارسنجی سمت سرور
            if (!ModelState.IsValid)
                return View(vm);

            // نگاشت ViewModel -> Entity
            var product = new Product
            {
                Id = vm.Id,
                Price = vm.Price,
                Title = vm.Title,
                ShortDescription=vm.ShortDescription,
            };

            // فراخوانی سرویس برای بروزرسانی
            var success = await _productService.EditProductAsync(product);

            if (!success)
            {
                // اگر مشتری پیدا نشد یا خطایی رخ داد
                ModelState.AddModelError("", "خطایی در بروزرسانی اطلاعات مشتری رخ داد.");
                return View(vm);
            }

            TempData["Success"] = "اطلاعات مشتری با موفقیت بروزرسانی شد.";
            return RedirectToAction(nameof(Index)); // برگرد به لیست مشتری‌ها
        }
        [HttpPost]
        public async Task<JsonResult> EditIsStatusProduct(bool isStatus, int productId)
        {
            var success = await _productService.EditIsStatusProdct(isStatus, productId);

            if (success)
            {
                return Json(new { success = true, message = "وضعیت محصول با موفقیت ویرایش شد." });
            }
            else
            {
                return Json(new { success = false, message = "خطا در ویرایش وضعیت محصول." });
            }
        }

        #endregion
    }
}
