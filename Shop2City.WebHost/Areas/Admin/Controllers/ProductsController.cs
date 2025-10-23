using MadWin.Application.DTOs.Products;
using MadWin.Core.Entities.CommissionRates;
using MadWin.Core.Entities.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop2City.Core.Services.Products;
using Shop2City.WebHost.ViewModels.CommissionRates;

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
                ProductId = product.ProductId,
                Price = product.Price,
                Title = product.Title,
            };

            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProductDto editproduct)
        {
            if (!ModelState.IsValid)
                return View(editproduct);
            var productEntity = new Product
            {
                Title = editproduct.Title,
                Price = editproduct.Price
            };

            var success = await _productService.EditProductAsync(editproduct);

            if (!success)
            {
                ModelState.AddModelError("", "خطایی در بروزرسانی اطلاعات محصول رخ داد.");
                return View(editproduct
                    
                    );
            }

            TempData["Success"] = "اطلاعات محصول با موفقیت بروزرسانی شد.";
            return RedirectToAction(nameof(Index));
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
