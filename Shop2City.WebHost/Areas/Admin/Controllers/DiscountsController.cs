using MadWin.Application.Services;
using MadWin.Core.DTOs.DisCounts;
using MadWin.Core.Entities.CommissionRates;
using MadWin.Core.Entities.Discounts;
using MadWin.Infrastructure.Convertors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop2City.WebHost.Dtos.Discounts;
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
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] AddDiscountForAdminViewModel model,string startDate,string expiryDate)
        {
            model.StartDate = ConvertDateTime.ToGreDateTime(startDate);
            model.ExpiryDate = ConvertDateTime.ToGreDateTime(expiryDate);
            var discount = new Discount
            {
                ExpiryDate=model.ExpiryDate,
                StartDate=model.StartDate,
                DiscountCode=model.DiscountCode,
                Item = model.Item,
                Percentage = model.Percentage,
                InitialCount=model.InitialCount,
                UseableCount = 0,
            };
            await _discountService.AddDiscount(discount);
            return RedirectToAction("Index");

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

        [HttpPost]
        public async Task<IActionResult> IsExistDisCountCode([FromBody] CheckDiscountDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                return BadRequest(new { message = "کد تخفیف الزامی است" });

            var exists = await _discountService.IsExistDisCountCode(dto.Code);

            return Ok(new { isDuplicate = exists });
        }

    }
}
