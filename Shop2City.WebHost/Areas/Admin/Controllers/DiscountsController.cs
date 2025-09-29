using MadWin.Application.Services;
using MadWin.Core.Entities.CommissionRates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop2City.WebHost.ViewModels.CommissionRates;

namespace Shop2City.WebHost.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class DiscountsController : Controller
    {
        public readonly IDiscountService _discountService;
        public DiscountsController(IDiscountService discountService)
        {
            _discountService = discountService;   
        }
        public async Task<IActionResult> Index(int pageId = 1)
        {
            var allDiscounts = await _discountService.GetAllDiscountsAsync(pageId);
            return View(allDiscounts);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var discounts = await _discountService.GetByIdAsync(id);
            if (discounts == null)
            {
                return NotFound();
            }

            var vm = new DiscountsEditViewModel
            {
                //Id = commissionRates.Id,
                //PartCount = commissionRates.PartCount,
                //IsEqualParts = commissionRates.IsEqualParts,
                //CommissionPercent = commissionRates.CommissionPercent,

            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DiscountsEditViewModel vm)
        {
            // اعتبارسنجی سمت سرور
            if (!ModelState.IsValid)
                return View(vm);

            // نگاشت ViewModel -> Entity
            var commissionRate = new CommissionRate
            {
                Id = vm.Id,
                PartCount = vm.PartCount,
                IsEqualParts = vm.IsEqualParts,
                CommissionPercent = vm.CommissionPercent,
            };

            // فراخوانی سرویس برای بروزرسانی
            var success = false;// await _commissionRatesService.EditCommissionRateAsync(commissionRate);

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
