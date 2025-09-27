using MadWin.Application.DTOs.CommissionRates;
using MadWin.Application.Services;
using MadWin.Core.Entities.CommissionRates;
using MadWin.Core.Entities.CurtainComponents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop2City.WebHost.ViewModels.CommissionRates;
using Shop2City.WebHost.ViewModels.CurtainComponents;

namespace Shop2City.WebHost.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class CommissionRatesController : Controller
    {
        public readonly ICommissionRatesService _commissionRatesService;

        public CommissionRatesController(ICommissionRatesService commissionRatesService)
        {
           _commissionRatesService = commissionRatesService; 
        }
        public async Task<IActionResult> Index()
        {
            var commissionRate = await _commissionRatesService.GetAllCommissionRateAsync();
            return View(commissionRate);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var commissionRates = await _commissionRatesService.GetByIdAsync(id);
            if (commissionRates == null)
            {
                return NotFound();
            }

            var vm = new DiscountsEditViewModel
            {
                Id = commissionRates.Id,
                PartCount = commissionRates.PartCount,
                IsEqualParts = commissionRates.IsEqualParts,
                CommissionPercent = commissionRates.CommissionPercent,

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
                PartCount=vm.PartCount,
                IsEqualParts=vm.IsEqualParts, 
                CommissionPercent = vm.CommissionPercent,
            };

            // فراخوانی سرویس برای بروزرسانی
            var success = await _commissionRatesService.EditCommissionRateAsync(commissionRate);

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
