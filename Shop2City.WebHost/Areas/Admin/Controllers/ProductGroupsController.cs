using MadWin.Application.Services;
using MadWin.Core.DTOs.ProductGroups;
using MadWin.Core.Entities.CommissionRates;
using MadWin.Core.Entities.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop2City.WebHost.ViewModels.CommissionRates;

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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var productGroup = await _productGroupService.GetByIdAsync(id);
            if (productGroup == null)
            {
                return NotFound();
            }

            var vm = new ProductGroupItemForAdminDto
            {
                Id = id,
                ParentId = productGroup.ParentId,
                Title= productGroup.Title
            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductGroupItemForAdminDto vm)
        {

            if (!ModelState.IsValid)
                return View(vm);

            var productGroup = new ProductGroup
            {
                Id = vm.Id,
                ParentId = vm.ParentId,
                Title = vm.Title
            };

            // فراخوانی سرویس برای بروزرسانی
            var success = await _productGroupService.EditProductGroupsAsync(productGroup);

            if (!success)
            {
                // اگر مشتری پیدا نشد یا خطایی رخ داد
                ModelState.AddModelError("", "خطایی در بروزرسانی اطلاعات مشتری رخ داد.");
                return View(vm);
            }

            TempData["Success"] = "اطلاعات مشتری با موفقیت بروزرسانی شد.";
            return RedirectToAction(nameof(Index)); // برگرد به لیست مشتری‌ها
        }
    }
}